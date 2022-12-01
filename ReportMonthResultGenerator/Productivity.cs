using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ReportMonthResultGenerator
{
    public class Productivity
    {
        public class ProductivityCalculatedValues
        {
            public Dictionary<Type, double?> Values = new Dictionary<Type, double?>()
            {
                {typeof(AutoCalc.ProductivBarista) ,null },
                {typeof(AutoCalc.ProductivSeller) ,null },
                {typeof(AutoCalc.ProductivCook) ,null }
            };
            //public double? Barista;
            //public double? Seller;
            //public double? Cook;
        }
        public static Dictionary<DateTime, Dictionary<int, ProductivityCalculatedValues>> CalculatedGoods = new Dictionary<DateTime, Dictionary<int, ProductivityCalculatedValues>>();
        //public static Dictionary<DateTime, Dictionary<int, ProductivityCalculatedValues>> CalculatedHours = new Dictionary<DateTime, Dictionary<int, ProductivityCalculatedValues>>();

        public static bool GestoryWeightsUpdatedToday = false;

        public static bool HasGoodsCalculatedForDay(DateTime day)
        {
            return CalculatedGoods.ContainsKey(day);// && CalculatedHours.ContainsKey(day);
        }

        public static void CalculateGoodsForDay(DateTime day)
        {
            // ******************************* Расчет отработанных часов (в производных классах AutoCalc)

            //Dictionary<int, ProductivityCalculatedValues> Hours = new Dictionary<int, ProductivityCalculatedValues>();


            //if (!CalculatedHours.ContainsKey(day))
            //    CalculatedHours.Add(day, Hours);
            //else
            //    CalculatedHours[day] = Hours;


            //var esetset = StaffBase.GetEmplsOfPos(new DateTime(day.Year, day.Month, 1), AutoCalc.ProductivityBase.WorkerCats[typeof(AutoCalc.ProductivCook)]);
            //;


            // ******************************* Обновление списка весовых бар-кодов




            //GestoryWeightsUpdatedToday = true;

            if (DateTime.Now == new DateTime(2022, 06, 16))
                GestoryWeightsUpdatedToday = true;

            if (!GestoryWeightsUpdatedToday)///////////////!!!!!!!!!!!!!!!!!!!!!!!!
            {
                List<GesItemsWeight> dbRes = new List<GesItemsWeight>();
                WebSrvSpisanie.Ges3ServicesUTF8ObjClient gesClient = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();
                WebSrvSpisanie.AllMenuForSite_T_barcRow[] result;

                Ges3.Ges3ServicesObjClient wClient = new Ges3.Ges3ServicesObjClient();

                try
                {
                    gesClient.AllMenuForSite(out result);

                    List<string> weightMeasures = new List<string>() { "г" };//,"кг"};

                    foreach (WebSrvSpisanie.AllMenuForSite_T_barcRow item in result)
                    {
                        int barCode;
                        if (int.TryParse(item.bar_cod, out barCode))
                        {
                            if ((item.vesovoy != null && item.vesovoy == 1)
                                ||
                               (item.weightNumb != null && item.weightNumb == "1" && item.weightText != null && weightMeasures.Contains(item.weightText)))
                            {
                                int? weightNumb = null;
                                if (!string.IsNullOrWhiteSpace(item.weightNumb))
                                    weightNumb = int.Parse(item.weightNumb);

                                int? portionRatio;
                                wClient.katalogWeigth(barCode, out portionRatio);
                                if (portionRatio == 0)
                                {
                                    // Try Find BarCode by Bracode_Location
                                    int? newCode;
                                    gesClient.Barcode_Location(barCode, out newCode, out string name, out string unit, out decimal? quan);
                                    if(newCode != null && newCode != 0)
                                    {
                                        wClient.katalogWeigth(newCode, out portionRatio);
                                        if (portionRatio == 0)
                                            portionRatio = null;
                                    }
                                    else
                                        portionRatio = null;
                                }

                                dbRes.Add(new GesItemsWeight()
                                {
                                    BarCode = barCode,
                                    Vesovoy = (byte?)item.vesovoy,
                                    WeightNumb = weightNumb,
                                    WeightText = item.weightText,
                                    PortionRatio = portionRatio
                                });
                            }
                        }
                    }

                    if (dbRes.Count() > 0)
                    {
                        var db = new ReportBaseDataContext();
                        db.GesItemsWeight.DeleteAllOnSubmit(db.GesItemsWeight.Where(a => true));
                        db.GesItemsWeight.InsertAllOnSubmit(dbRes);
                        db.SubmitChanges();
                    }
                    //var iii = db.GesItemsWeight.Where(a => true).ToList();
                }
                catch (Exception ex)
                {
                    Utils.ToLog("Ошибка при расчете производительности труда (обновление списка весовых из Gestori) за " + day.ToShortDateString() + "    Err: " + ex.Message, true);
                }
                GestoryWeightsUpdatedToday = true;
            }

            // ******************************* Расчет произведенных товаров


            if (!CalculatedGoods.ContainsKey(day))
                CalculatedGoods.Add(day, new Dictionary<int, ProductivityCalculatedValues>());

            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            string q = string.Format(
            "   if object_id('tempdb..#TMP_VAL_FOR_DATE') is not null " +
            "       drop table #TMP_VAL_FOR_DATE " +
            "	SELECT " +
            "	   vall.[КодПодразд] as Dep, " +
            "	   vall.[Колич] as Quan, " +
            "	   cast((vall.[Колич] * ISNULL(DishCoeffs.[Value],1)) as float) as QuanKoef, " +
            "	   MenuITM.[Category] as CatId, " +
            "      Employee.[Position_ID] as EmpPosId, " +
            "      (CASE WHEN (NOT ItemWeight.[BarCode] IS NULL)OR(ItemWeight.[BarCode] IS NULL AND vall.[Колич]>@P4   /* AND 1=0   !!!!!!! */) THEN 1 ELSE 0 END) as IsVes, " +
            "          cast((cast((CASE  " +
            "              WHEN (NOT ItemWeight.[BarCode] IS NULL)OR(ItemWeight.[BarCode] IS NULL AND vall.[Колич]>@P4) " +
            "              THEN (CASE WHEN (IsNull(ItemWeight.[PortionRatio],@P5)<>0) THEN  " +
            "                        (CASE WHEN vall.[Колич]<>1.0 " +
            "                              THEN (SELECT MAX(X) FROM(VALUES(1),(FLOOR((vall.[Колич]+1.0)/IsNull(ItemWeight.[PortionRatio],@P5)))) T(X)) " +
            "                              ELSE 0 END) ELSE 1 END) 	 " +
            ////"              ELSE vall.[Колич]  " +
            "              ELSE cast((vall.[Колич]) as float)  " +
            "           END) as float)*ISNULL(DishCoeffs.[Value],1))as float) as QuanRecalc " +
            //"      ItemWeight.[PortionRatio]as PortionRatio " +
            //"      (CASE WHEN NOT ItemWeight.[BarCode] IS NULL THEN (CASE WHEN IsNull(ItemWeight.[PortionRatio],@P5)<>0 THEN FLOOR((vall.[Колич]+1.0)/ItemWeight.[PortionRatio]) ELSE 1 END) ELSE vall.[Колич] END) as QuanRecalc " +
            "	INTO #TMP_VAL_FOR_DATE " +
            "    FROM [btest].[dbo].[vall] as vall  " +
            "    LEFT JOIN [S2010].[Diogen].[dbo].[AlohaMenuITM] as MenuITM on MenuITM.ID = vall.[БарКод] and MenuITM.Dep = vall.[КодПодразд] " +
            "    LEFT JOIN [S2010].[Diogen].[dbo].[StaffEmployee] as Employee on Employee.[EMPLOYEE_ID] = vall.[НомерОфицианта] and Employee.[SUBDIVISION_ID] = vall.[КодПодразд] " +
            "    LEFT JOIN [S2010].[Diogen].[dbo].[GesItemsWeight] as ItemWeight on ItemWeight.[BarCode] = vall.[БарКод]        /* AND 1=0  !!!!!!! */ " +
            "    LEFT JOIN [S2010].[Diogen].[dbo].[DishCoeffsKissTheCook] as DishCoeffs on DishCoeffs.[Barcod] = vall.[БарКод]     /*  AND 1=0   !!!!!!! */ " +
            "    where " +
            "       vall.[Число] = @P3 " +
            "       and vall.[Год] = @P1 " +
            "	   and vall.[Месяц] = @P2 " +
            "	   and not [Колич] is null " +
            "       and [СуммаИтог]!=0  " +
            //"      and [Колич] < @P4 " +
            //"      and not(ItemWeight.[BarCode] is null and vall.[Колич] > @P4) " +
            "	   and not MenuITM.[Category] is null " +
            "	SELECT  " +
            "	    deps.Dep, " +
            "		(SELECT SUM(cast(Quan as float)) FROM #TMP_VAL_FOR_DATE as dat WHERE dat.Dep=deps.Dep AND CatId in({0}) AND IsVes=0) as QuanBarista, " +
            "		(SELECT SUM(cast(Quan as float)) FROM #TMP_VAL_FOR_DATE as dat WHERE dat.Dep=deps.Dep AND CatId in({1}) AND IsVes=0 /*and IsNull(EmpPosId,0)IN({4})*/) as QuanSeller, " +
            "       (SELECT SUM(cast((CASE WHEN IsVes=0     and 0=1 /* !!!!!!! */ THEN QuanKoef ELSE QuanRecalc END) as float)) FROM #TMP_VAL_FOR_DATE as dat WHERE dat.Dep=deps.Dep AND    NOT    CatId in({2})) as QuanCook " +
            //"		(SELECT SUM(cast(Quan as float)) FROM #TMP_VAL_FOR_DATE as dat WHERE dat.Dep=deps.Dep AND CatId in({2}) AND IsVes=0) as QuanCook " +
            ////"       (SELECT SUM(cast((CASE WHEN IsVes=0 THEN Quan ELSE QuanRecalc END) as float)) FROM #TMP_VAL_FOR_DATE as dat WHERE dat.Dep=deps.Dep AND CatId in(9,10,11,13,14,15,16,23,27,29,30,31,33,34,35,57)) as QuanCook, " +            
            "	FROM (SELECT DISTINCT Dep FROM #TMP_VAL_FOR_DATE) as deps " +
            "	DROP TABLE #TMP_VAL_FOR_DATE ",
            string.Join(",", AutoCalc.ProductivityBase.GoodsCats[typeof(AutoCalc.ProductivBarista)]), // 0
            string.Join(",", AutoCalc.ProductivityBase.GoodsCats[typeof(AutoCalc.ProductivSeller)]), // 1
            string.Join(",", AutoCalc.ProductivityBase.GoodsCatsExclude[typeof(AutoCalc.ProductivCook)]), // 2
            string.Join(",", AutoCalc.ProductivityBase.WorkerCats[typeof(AutoCalc.ProductivBarista)]), // 3 - NOT USED
            string.Join(",", AutoCalc.ProductivityBase.WorkerCats[typeof(AutoCalc.ProductivSeller)]), // 4 - NOT USED
            string.Join(",", AutoCalc.ProductivityBase.WorkerCats[typeof(AutoCalc.ProductivCook)]) // 5 - NOT USED
            );

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc2 = new SqlCommand(q, conn);
            Sc2.CommandTimeout = 100000;
            SqlParameter P1 = new SqlParameter("P1", day.Year);
            SqlParameter P2 = new SqlParameter("P2", day.Month);
            SqlParameter P3 = new SqlParameter("P3", day.Day);
            SqlParameter P4 = new SqlParameter("P4", AutoCalc.ProductivityBase.VesCountLimit);
            SqlParameter P5 = new SqlParameter("P5", AutoCalc.ProductivityBase.DefaultPortionRatio);
            //SqlParameter P4 = new SqlParameter("P4", string.Join(",", GoodsOfBarista));
            //SqlParameter P5 = new SqlParameter("P5", string.Join(",", GoodsOfSeller));
            //SqlParameter P6 = new SqlParameter("P6", string.Join(",", GoodsOfCook));
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.Parameters.Add(P3);
            Sc2.Parameters.Add(P4);
            Sc2.Parameters.Add(P5);
            //Sc2.Parameters.Add(P4);
            //Sc2.Parameters.Add(P5);
            //Sc2.Parameters.Add(P6);
            Sc2.CommandTimeout = 0;

            string sss = Sc2.CommandText;

            SqlDataReader Sr2 = Sc2.ExecuteReader();
            List<int> ChkCount = new List<int>();
            while (Sr2.Read())
            {
                try
                {
                    int Dep = Sr2.GetInt32(0);
                    ProductivityCalculatedValues Rec = new ProductivityCalculatedValues();
                    if (!Convert.IsDBNull(Sr2.GetValue(1)))
                        Rec.Values[typeof(AutoCalc.ProductivBarista)] = Sr2.GetDouble(1);
                    if (!Convert.IsDBNull(Sr2.GetValue(2)))
                        Rec.Values[typeof(AutoCalc.ProductivSeller)] = Sr2.GetDouble(2);
                    if (!Convert.IsDBNull(Sr2.GetValue(3)))
                        Rec.Values[typeof(AutoCalc.ProductivCook)] = Sr2.GetDouble(3);

                    if (!CalculatedGoods[day].ContainsKey(Dep))
                        CalculatedGoods[day].Add(Dep, Rec);
                    else
                        CalculatedGoods[day][Dep] = Rec;
                }
                catch(Exception ex)
                {
                    Utils.ToLog("Ошибка при расчете производительности труда (запрос в произведенных товарах в кубы) за " + day.ToShortDateString() + "    Err: " + ex.Message, true);
                }

            }
            Sr2.Close();

            conn.Close();


            //var testttt = CalculatedGoods[day].First(_data => _data.Key == 205);

            CalculatedGoods[day] = AutoCalc.CorrectTwinDep.Correct(CalculatedGoods[day]);
        }

        public static List<ReportDayResult> CalcForDay(DateTime day, AutoCalc.ProductivityBase sender)
        {
            var resOut = new List<ReportDayResult>();

            int TypeId = sender.GetTypeId();
            Type _type = sender.GetType();
            
            ReportMonthResultGenerator.StaffEmployeeParameters.StaffEmployeeParametersObjClient staffClient = new ReportMonthResultGenerator.StaffEmployeeParameters.StaffEmployeeParametersObjClient();

            // Границы дат
            //DateTime Fdt0 = day;
            //DateTime Edt0 = new DateTime(day.Ticks).AddDays(1);
            //Edt0 = new DateTime(Edt0.Year, Edt0.Month, Edt0.Day, 0, 0, 0, 0);


            List<CEmplWt> Wts = new List<CEmplWt>();
            System.Data.Odbc.OdbcConnection conn = StaffBase.ConnectionOpen();
            var empls = StaffBase.GetEmplsOfPos(new DateTime(day.Year, day.Month, 1), AutoCalc.ProductivityBase.WorkerCats[_type], true, conn);
            Wts.AddRange(StaffBase.GetWts(empls, day, day.AddDays(1), true, conn));
            StaffBase.ConnectionClose(conn);

            try
            {

                var Serv = new S2010.XrepSoapClient();
                var DepList = Serv.GetPointList3();
                foreach (S2010.DepartmentInfo Dii in DepList)
                {
                    //if (Dii.Number != 111 && Dii.Number != 114 && Dii.Number != 121)
                    //    continue;
                    if (Dii.Enabled)
                    {
                        //Dictionary<int, StaffEmployeeParameters.working_time_T_wrkRow[]> workingResult = new Dictionary<int, StaffEmployeeParameters.working_time_T_wrkRow[]>();

                        //List<object> debugInfo = new List<object>();

                        //decimal? hours0 = 0;
                        decimal hours = 0;

                        double resHours = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (StaffWtToExcel.GetMaxDate(a.StopDt, day) - StaffWtToExcel.GetMinDate(a.StartDt, day.AddDays(1))).TotalHours);
                        hours += (decimal)resHours;

                        /*foreach (int workerCat in AutoCalc.ProductivityBase.WorkerCats[_type])
                        {
                            StaffEmployeeParameters.working_time_T_wrkRow[] resRows;
                            staffClient.working_time(day.AddDays(-1), day.AddDays(1), Dii.Number, workerCat, out resRows);

                            foreach (StaffEmployeeParameters.working_time_T_wrkRow row in resRows)
                                if (!((row.dtm_st < Fdt0 && row.dtm_fn < Fdt0) || (row.dtm_st > Edt0 && row.dtm_fn > Edt0)))
                                {
                                    // Отрезать часы, выпадающие из суток
                                    DateTime Fdt = (DateTime)(row.dtm_st >= Fdt0 ? row.dtm_st : Fdt0);
                                    DateTime Edt = (DateTime)(row.dtm_fn <= Edt0 ? row.dtm_fn : Edt0);
                                    TimeSpan DateDiff = Edt.Subtract(Fdt);
                                    decimal hour = (decimal)DateDiff.Hours + (decimal)((decimal)DateDiff.Minutes / 60) + (decimal)((decimal)DateDiff.Seconds / 3600);

                                    if (row.q_ch != null)
                                    {
                                        //hours0 += row.q_ch;
                                        hours += hour;
                                    }
                                    //debugInfo.Add(new DebugInfo()
                                    //{
                                    //    Sdt0 = row.dtm_st,
                                    //    Edt0 = row.dtm_fn,
                                    //    quan0 = row.q_ch,
                                    //    Sdt=Fdt,
                                    //    Edt=Edt,
                                    //    quan=hour
                                    //});

                                }
                                else
                                {
                                    ;// Two dates out of range
                                }
                        }*/

                        ;

                        double? goods = null;

                        if (Productivity.CalculatedGoods[day].ContainsKey(Dii.Number))
                            goods = Productivity.CalculatedGoods[day][Dii.Number].Values[_type];

                        //if(TypeId == 19)
                        //{
                        //    var mozgData = MOZGIntegration.GetRestData(day, Dii.Number);
                        //    if (mozgData != null)
                        //        goods = mozgData.DishesCount;
                        //}

                        if (goods != null)
                        {
                            var res = new ReportDayResult()
                            {
                                BD = day,
                                TypeId = TypeId,
                                Dep = Dii.Number,
                                DepName = Dii.Name,
                                Count = (double)hours,
                                Summ = (double)goods,
                                Value = hours != 0 ? (double)((goods) / (double)hours) : 0
                            };
                            resOut.Add(res);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Utils.ToLog("Ошибка при расчете производительности труда (запрос об отработанных часах в стаф) за " + day.ToShortDateString() + "    Err: " + ex.Message, true);
            }
            return resOut;
        }
    }
}
