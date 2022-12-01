using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class SpisanieBase : CalcBase
    {
        /*    ЭТО ТОЖЕ УБРАТЬ ToDo */
        public static Dictionary<DateTime, List<SpisanieType>> MonthHasCalculatedToday = new Dictionary<DateTime, List<SpisanieType>>();
        /* */
        public enum SpisanieType { Dishes = 'F', Drinks = 'B', Desert = 'P', Alco = 'A' }
        public SpisanieType CalcType;
        public int GetTypeId()
        {
            return TypeId;
        }
        public override List<ReportDayResult> Calc(DateTime day)
        {
            string strCalcType = ((char)CalcType).ToString();
            var resOut = new List<SpisPercent>();

            /*    ЭТО ТОЖЕ УБРАТЬ ToDo */
            //DateTime dayFirstInMonth = new DateTime(day.Year, day.Month, 1);

            //if (!MonthHasCalculatedToday.ContainsKey(dayFirstInMonth))
            //    MonthHasCalculatedToday.Add(dayFirstInMonth, new List<SpisanieType>());

            //if (MonthHasCalculatedToday[dayFirstInMonth].Contains(CalcType))
            //{
            //    Utils.ToLog($"  Canceled: повторный расчет списания ({CalcType.ToString()}) за " + day.ToShortDateString(), true);
            //    return null;
            //}
            /* */

            ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient client = new ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient();

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
                        ////if (!(new int[]{ 310,280,321,270,380,290,190,255,104,235,205,180,395,375,285}).Contains(Dii.Number)) continue;
                        //if (Dii.Number != 310) continue;
                       // if (Dii.Number != 111 && Dii.Number != 114 && Dii.Number != 121) continue;
                        decimal? value;
                        decimal? valueSpisRub;
                        decimal? valueProdRub;

                        //int DiiNumTmp = Dii.Number; // ToDo - убрать это алкогольное после получения норм. данных из Джестори
                        //if (Dii.Number == 111 && CalcType == SpisanieType.Alco)
                        //    DiiNumTmp = 121;

                        //client.AnalysVSK_RX(strCalcType, /*DiiNumTmp,*/ Dii.Number, /*dayFirstInMonth,*/ day, day, out value, 
                        //    out decimal? pp_sp32, out decimal? pp_sp33, out decimal? pp_sp34, out decimal? pp_sp35, out decimal? pp_sp37, out decimal? pp_sp38, out decimal? pp_sp39, out decimal? pp_inv_N,
                        //    out decimal? Qsp32, out decimal? Qsp33, out decimal? Qsp34, out decimal? Qsp35, out decimal? Qsp37, out decimal? Qsp38, out decimal? Qsp39,
                        //    out valueSpisRub, out decimal? Stortage, out valueProdRub, out decimal? sales_RX);

                        //client.AnalysFBP1(strCalcType, /*DiiNumTmp,*/ Dii.Number, /*dayFirstInMonth,*/ day, day, out value,
                        //    out decimal? pp_sp32, out decimal? pp_sp33, out decimal? pp_sp34, out decimal? pp_sp35, out decimal? pp_sp37, out decimal? pp_sp38, out decimal? pp_sp39, out decimal? pp_sp391, out decimal? pp_sp392, out decimal? pp_inv_N,
                        //    out valueSpisRub, out decimal? Stortage, out valueProdRub);


                        DateTime day0 = new DateTime(day.Year, day.Month, 1);
                        client.AnalysFBP1_TAB(strCalcType, /*DiiNumTmp,*/ Dii.Number, /*dayFirstInMonth,*/ day0, day, out Ges3.AnalysFBP1_TAB_T_datesRow[] result, out value,
                                out decimal? pp_sp32, out decimal? pp_sp33, out decimal? pp_sp34, out decimal? pp_sp35, out decimal? pp_sp37, out decimal? pp_sp38, out decimal? pp_sp39, out decimal? pp_sp391, out decimal? pp_sp392, out decimal? pp_inv_N,
                                out valueSpisRub, out decimal? Stortage, out valueProdRub);

                        decimal spis = 0;
                        decimal sp_inv = 0;
                        decimal sales = 0;
                        for (DateTime dt = day0; dt <= day; dt = dt.AddDays(1))
                        //if ((valueProdRub != null && valueProdRub != 0)
                        //    ||(Dii.Number == 111 && (CalcType == SpisanieType.Alco || CalcType == SpisanieType.Drinks)))
                        {
                            var resDate = result.FirstOrDefault(_res => _res.DDate == dt);
                            if(resDate != null)
                            {
                                spis = resDate.Spis ?? 0;
                                sp_inv = resDate.Stortage ?? 0;
                                sales = resDate.AllSales ?? 0;
                            }
                            else
                            {
                                spis = 0;
                                sp_inv = 0;
                                sales = 0;
                            }
                            ////// !!!!!!!!!!!!!!!!!!!!!! убрать это !!!
                            ////Console.WriteLine($" {DateTime.Now.ToString(@"dd/MM/yyyy HH:mm:ss")}   {Dii.Number} {Dii.Name}   -   {valueSpisRub} / {valueProdRub} = {value} % ");
                            //Console.WriteLine($" {DateTime.Now.ToString(@"dd/MM/yyyy HH:mm:ss")}   {Dii.Number} {Dii.Name}   -   {(spis + sp_inv)} / {sales} ");

                            // !!! обновление данных всего месяца - ToDo убрать это !!!

                            //for (int iDay = 1; iDay <= day.Day; iDay++)
                            //{
                            var res = new SpisPercent()
                            {                
                                BD  = dt,
                                Dep = Dii.Number,
                                DepName = Dii.Name,
                                Producted = (double)sales,//1,//(double)value,
                                WrittenOff = (double)(spis + sp_inv)//value,
                            };



                            //var res = new SpisPercent()
                            //    {                                    
                            //        Dep = Dii.Number,
                            //        DepName = Dii.Name,
                            //        Producted = (double)valueProdRub,//1,//(double)value,
                            //        WrittenOff = (double)valueSpisRub//value,
                            //    };
                            resOut.Add(res);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ToLog($"Ошибка при расчете доли списания ({CalcType.ToString()}) за " + day.ToShortDateString() + "    Err: " + ex.Message, true);
            }
            /*    ЭТО ТОЖЕ УБРАТЬ ToDo */
            //MonthHasCalculatedToday[dayFirstInMonth].Add(CalcType);
            /* */
            {
                var res = CorrectTwinDep.Correct(resOut);
                List<ReportDayResult> resFinal = res.Select(a => new ReportDayResult()
                {
                    BD = a.BD,//day,
                    Count = a.Producted, //a.Checks,
                    Dep = a.Dep,
                    DepName = a.DepName,
                    Summ = a.WrittenOff, //a.Value,
                    Value = a.Value, // a.ValueOnCheck,
                    TypeId = TypeId
                }).ToList();

                return resFinal;
            }
            //Utils.ToLog($"ОкNull ({CalcType.ToString()}) TypeId={TypeId} за " + day.ToShortDateString(), true);
            //return null;
        }
    }
}
