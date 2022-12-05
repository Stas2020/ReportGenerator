using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    public static class Calculation
    {

        private static List<CalcBase> calculationsList = new List<CalcBase>();
        
        public static void InitOnlySpis()
        {
            calculationsList.Add(new AutoCalc.SpisDesert());
            calculationsList.Add(new AutoCalc.SpisAlco());
            calculationsList.Add(new AutoCalc.SpisDishes());
            calculationsList.Add(new AutoCalc.SpisDrinks());
        }
       
        public enum InitMode { Common, Spis, MozgOnly, OnlineShopOnly }
        public static void Init(DateTime? _minDate = null, InitMode _initMode = InitMode.Common)
        {

            ////calculationsList.Add(new AutoCalc.DrinksCalc());
            ////calculationsList.Add(new AutoCalc.DishesCalc());

            ////calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            ////calculationsList.Add(new AutoCalc.DishesCalcGuest());


            ////calculationsList.Add(new AutoCalc.OrderTimeCalc());
            ////calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());


            //calculationsList.Add(new AutoCalc.OrderTimeCalc());
            //calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());
            //return;

            //switch (_initMode)
            //{
            //    //case InitMode.Common:
            //    //    break;
            //    case InitMode.Spis:
            //        calculationsList.Add(new AutoCalc.SpisDesert());
            //        calculationsList.Add(new AutoCalc.SpisAlco());
            //        calculationsList.Add(new AutoCalc.SpisDishes());
            //        calculationsList.Add(new AutoCalc.SpisDrinks());
            //        return;
            //    case InitMode.MozgOnly:
            //        calculationsList.Add(new AutoCalc.DrinksCalc());
            //        calculationsList.Add(new AutoCalc.DishesCalc());

            //        calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            //        calculationsList.Add(new AutoCalc.DishesCalcGuest());
            //        return;
            //    default:
            //        break;
            //}


            //calculationsList.Add(new AutoCalc.ProductivBarista());
            //calculationsList.Add(new AutoCalc.ProductivSeller());
            //calculationsList.Add(new AutoCalc.ProductivCook());
            //return;

            //calculationsList.Add(new AutoCalc.OnlineStoreNegative(_minDate));
            //return;
            //calculationsList.Add(new AutoCalc.ProductivCook());
            //return;




            calculationsList.Add(new AutoCalc.SpisDesert());
            calculationsList.Add(new AutoCalc.SpisAlco());
            calculationsList.Add(new AutoCalc.SpisDishes());
            calculationsList.Add(new AutoCalc.SpisDrinks());
            return;

            //calculationsList.Add(new AutoCalc.RashMatCalc());

            //calculationsList.Add(new AutoCalc.OrderTimeCalc());
            //calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());

            //calculationsList.Add(new AutoCalc.OnlineStoreNegative(_minDate));

            //перезагрузка данных рейтигна ИМ
            //calculationsList.Add(new AutoCalc.OnlineStoreRating(_minDate));
            //return;

            //2022-10-10 перезагрузка данных по доле списания алкоголя
            //calculationsList.Add(new AutoCalc.SpisAlco());

            //2022-10-14 перезагрузка данных по доле списания блюд
            //calculationsList.Add(new AutoCalc.SpisDishes());

            //2022-10-17 расходные материалы
            //calculationsList.Add(new AutoCalc.RashMatCalc());
            //calculationsList.Add(new AutoCalc.RashMatGroup1Calc());
            //calculationsList.Add(new AutoCalc.RashMatGroup2Calc());
            //return;

            //2022-10-18 продажа напитков и продажа блюд на гостя
            //calculationsList.Add(new AutoCalc.DishesCalcGuest());
            //calculationsList.Add(new AutoCalc.DrinksCalcGuest());

            //2022-10-20 негативные отзывы ИМ
            //calculationsList.Add(new AutoCalc.OnlineStoreNegative(_minDate));

            //calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            //calculationsList.Add(new AutoCalc.DishesCalcGuest());

            //calculationsList.Add(new AutoCalc.OnlineStoreRating(_minDate));
            //return;


            if (_initMode == InitMode.MozgOnly)
            {
                calculationsList.Add(new AutoCalc.OnlineStoreNegative(_minDate));

                calculationsList.Add(new AutoCalc.DrinksCalc());
                calculationsList.Add(new AutoCalc.DishesCalc());

                calculationsList.Add(new AutoCalc.DrinksCalcGuest());
                calculationsList.Add(new AutoCalc.DishesCalcGuest());

                ////////calculationsList.Add(new AutoCalc.ProductivCook());

                return;
            }

            if (_initMode == InitMode.OnlineShopOnly)
            {
                calculationsList.Add(new AutoCalc.OnlineStoreRating(_minDate));

                return;
            }

            //calculationsList.Add(new AutoCalc.OrderTimeCalc());
            //calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());
            //return;

            //calculationsList.Add(new AutoCalc.ProductivBarista());
            //calculationsList.Add(new AutoCalc.ProductivSeller());
            //calculationsList.Add(new AutoCalc.ProductivCook());



            //calculationsList.Add(new AutoCalc.ProductivBarista());
            //calculationsList.Add(new AutoCalc.ProductivSeller());
            //calculationsList.Add(new AutoCalc.ProductivCook());
            //return;


            //////calculationsList.Add(new AutoCalc.DrinksCalc()); - это в режиме МОЗГ
            //////calculationsList.Add(new AutoCalc.DishesCalc());

            //////calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            //////calculationsList.Add(new AutoCalc.DishesCalcGuest());

            calculationsList.Add(new AutoCalc.OnlineStoreRating(_minDate));
            // return;

            calculationsList.Add(new AutoCalc.ShiftSpeesCalc());
            calculationsList.Add(new AutoCalc.Term5Percent());

            calculationsList.Add(new AutoCalc.OrderTimeCalc());
            calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());

            //calculationsList.Add(new AutoCalc.DesertSpisCalc());

            calculationsList.Add(new AutoCalc.SpisDesert());
            calculationsList.Add(new AutoCalc.SpisAlco());
            calculationsList.Add(new AutoCalc.SpisDishes());
            calculationsList.Add(new AutoCalc.SpisDrinks());


            calculationsList.Add(new AutoCalc.ProductivBarista());
            calculationsList.Add(new AutoCalc.ProductivSeller());
            calculationsList.Add(new AutoCalc.ProductivCook());


            calculationsList.Add(new AutoCalc.RashMatCalc());
            //разбиение на 2 показателя
            calculationsList.Add(new AutoCalc.RashMatGroup1Calc());
            calculationsList.Add(new AutoCalc.RashMatGroup2Calc());
           
        }
        public static void DeleteSpis()
        {
            calculationsList.RemoveAll(_spis => (_spis is AutoCalc.SpisDesert) || (_spis is AutoCalc.SpisAlco) || (_spis is AutoCalc.SpisDishes) || (_spis is AutoCalc.SpisDrinks));
        }
        public static void InitForDebug_08_06_21()
        {


            //calculationsList.Add(new AutoCalc.ShiftSpeesCalc());

            //calculationsList.Add(new AutoCalc.OrderTimeCalc());
            //calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());

            //calculationsList.Add(new AutoCalc.SpisDesert());
            //calculationsList.Add(new AutoCalc.SpisAlco());
            //calculationsList.Add(new AutoCalc.SpisDishes());
            //calculationsList.Add(new AutoCalc.SpisDrinks());

            //calculationsList.Add(new AutoCalc.RashMatCalc());

            ////calculationsList.Add(new AutoCalc.DrinksCalc());
            ////calculationsList.Add(new AutoCalc.DishesCalc());

            ////////////calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            ////////////calculationsList.Add(new AutoCalc.DishesCalcGuest());

            //calculationsList.Add(new AutoCalc.ProductivBarista());
            //calculationsList.Add(new AutoCalc.ProductivSeller());
            //calculationsList.Add(new AutoCalc.ProductivCook());

            //calculationsList.Add(new AutoCalc.SpisAlco());




            //calculationsList.Add(new AutoCalc.ShiftSpeesCalc());

            //calculationsList.Add(new AutoCalc.OrderTimeCalc());
            //calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());

            ////calculationsList.Add(new AutoCalc.DesertSpisCalc());
            //calculationsList.Add(new AutoCalc.SpisDesert());
            //calculationsList.Add(new AutoCalc.SpisAlco());
            //calculationsList.Add(new AutoCalc.SpisDishes());
            //calculationsList.Add(new AutoCalc.SpisDrinks());

            //calculationsList.Add(new AutoCalc.RashMatCalc());

            //calculationsList.Add(new AutoCalc.DrinksCalc());
            //calculationsList.Add(new AutoCalc.DishesCalc());

            //calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            //calculationsList.Add(new AutoCalc.DishesCalcGuest());

            //calculationsList.Add(new AutoCalc.ProductivBarista());
            //calculationsList.Add(new AutoCalc.ProductivSeller());
            //calculationsList.Add(new AutoCalc.ProductivCook());







            //calculationsList.Add(new AutoCalc.ShiftSpeesCalc());
            //calculationsList.Add(new AutoCalc.DrinksCalc());
            //calculationsList.Add(new AutoCalc.RashMatCalc());
            //calculationsList.Add(new AutoCalc.DesertSpisCalc());


            ////calculationsList.Add(new AutoCalc.ShiftSpeesCalc());

            //calculationsList.Add(new AutoCalc.OrderTimeCalc());
            //calculationsList.Add(new AutoCalc.OrderTimeWODeliveryCalc());

            ////calculationsList.Add(new AutoCalc.DesertSpisCalc());

            ////Utils.ToDebugLog($"InitForDebug_08_06_21 start");
            ////calculationsList.Add(new AutoCalc.RashMatCalc());
            ////Utils.ToDebugLog($"InitForDebug_08_06_21 end");

            ////calculationsList.Add(new AutoCalc.DrinksCalc());
            ////calculationsList.Add(new AutoCalc.DishesCalc());


            Utils.ToDebugLog($"InitForDebug_ start");

            //calculationsList.Add(new AutoCalc.RashMatCalc());

            //calculationsList.Add(new AutoCalc.SpisAlco());
            //calculationsList.Add(new AutoCalc.SpisDesert());
            //calculationsList.Add(new AutoCalc.SpisDishes());
            //calculationsList.Add(new AutoCalc.SpisDrinks());

            ////////calculationsList.Add(new AutoCalc.ProductivBarista());
            //calculationsList.Add(new AutoCalc.ProductivSeller());


            //calculationsList.Add(new AutoCalc.ProductivCook());

            calculationsList.Add(new AutoCalc.DrinksCalc());
            calculationsList.Add(new AutoCalc.DishesCalc());
            calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            calculationsList.Add(new AutoCalc.DishesCalcGuest());

            //calculationsList.Add(new AutoCalc.DishesCalc());
            //calculationsList.Add(new AutoCalc.DishesCalcGuest());

            //calculationsList.Add(new AutoCalc.DrinksCalc());
            //calculationsList.Add(new AutoCalc.DrinksCalcGuest());


            ////calculationsList.Add(new AutoCalc.DrinksCalcGuest());
            Utils.ToDebugLog($"InitForDebug_ end");
        }
        public static void DayCalc(DateTime day)
        {
            foreach (var c in calculationsList)
            {
                string s = $"{DateTime.Now} DayCalc date {day.ToShortDateString()} type: {c.TypeCaption}";
                Console.WriteLine(s);
                Utils.ToLog(s);
                c.InsertData(day);
            }
        }
        public static void MonthCalc(DateTime month)
        {
            string s = $"{DateTime.Now} MonthCalc date {month.Month:00}.{month.Year} allTypeReports";
            Console.WriteLine(s);
            Utils.ToLog(s);
            foreach (var c in calculationsList)
            {
                //string s = $"{DateTime.Now} MonthCalc date {month.Month}.{month.Year} type: {c.TypeCaption}";
                ////Console.WriteLine(s);
                //Utils.ToLog(s, true);
                c.RecalcMonth(month);
            }
        }
    }

    public abstract class CalcBase
    {
        //TypeId вычислений доли списания
        private static int[] _spisTypeIds = new int[] { 20, 21, 22, 23,};

        public CalcBase()
        { 
        
        }

        int typeId = 0;
        protected int TypeId {
            get
            {
                if (typeId == 0)
                {
                    var db = new ReportBaseDataContext();
                    string cName = this.GetType().ToString();
                    cName = cName.Substring(cName.LastIndexOf(".")+1);
                    typeId = (db.ReportTypeIds.FirstOrDefault(a => a.TypeSystemName == cName)?.TypeId).GetValueOrDefault();
                }
                return typeId;
            }
        }

        public  string TypeCaption
        {
            get
            {
                    var db = new ReportBaseDataContext();
                    string cName = this.GetType().ToString();
                    cName = cName.Substring(cName.LastIndexOf(".") + 1);
                    return  db.ReportTypeIds.FirstOrDefault(a => a.TypeSystemName == cName)?.Caption;
       
            }
        }

        public abstract List<ReportDayResult> Calc(DateTime date);

        public void InsertData(DateTime date)
        {
            ReportLogger.UpdateDay(date, TypeId);

            var res = Calc(date);
            //return;
            if (res != null)
            {
                var db = new ReportBaseDataContext();

                //// КОСТЫЛЬ ДЛЯ ДОЛИ СПИСАНИЯ - ОБНУЛЕНИЕ МЕСЯЦА - ToDo - убрать это
                //if (this is SpisAlco || this is SpisDesert || this is SpisDishes || this is SpisDrinks)
                //{
                //    List<DateTime> dates = res.Where(_res => _res.BD != null).Select(_res => (DateTime)_res.BD).Distinct().ToList();
                //    if (db.ReportDayResult.Any(a => a.BD != null && dates.Contains(a.BD.Value) && a.TypeId == TypeId))
                //    {
                //        var del = db.ReportDayResult.Where(a => a.BD != null && dates.Contains(a.BD.Value) && a.TypeId == TypeId);
                //        db.ReportDayResult.DeleteAllOnSubmit(del);
                //        db.SubmitChanges();
                //    }
                //}
                //// Так было
                //else
                //{

                DateTime day0 = date.Date; // Убрать часы и минуты

                if (_spisTypeIds.Contains(typeId)) // Для списаний перезаписывать за весь период
                    day0 = new DateTime(date.Year, date.Month, 1).Date;

                //2022-10-11 убран фильтр по подразделению  && (a.Dep == 193)
                //if (db.ReportDayResult.Any(a => a.BD >= date && a.BD <= date && a.TypeId == TypeId && (a.Dep == 193) /* && (a.Dep == 310) */ /*  && (a.Dep == 111 || a.Dep==121 || a.Dep==114)    */        )) //2022-10-11
                if (db.ReportDayResult.Any(a => a.BD >= day0 && a.BD <= date && a.TypeId == TypeId /* && (a.Dep == 193) */ /* && (a.Dep == 310) */ /*  && (a.Dep == 111 || a.Dep==121 || a.Dep==114)    */        ))
                {
                    //2022-10-11 убран фильтр по подразделению  && (a.Dep == 193)
                    //var del = db.ReportDayResult.Where(a => a.BD >= date && a.BD <= date && a.TypeId == TypeId  && (a.Dep == 193)/* && (a.Dep == 310) */  /*     && (a.Dep == 111 || a.Dep==121 || a.Dep==114)    */      ); //2022-10-11
                    var del = db.ReportDayResult.Where(a => a.BD >= day0 && a.BD <= date && a.TypeId == TypeId  /* && (a.Dep == 193) */ /* && (a.Dep == 310) */  /*     && (a.Dep == 111 || a.Dep==121 || a.Dep==114)    */      );

                    db.ReportDayResult.DeleteAllOnSubmit(del);
                    db.SubmitChanges();
                }
                //}
                // Ужасный костыль из-за смены подразделений !!!!! убрать потом!!!!!!!!!!!!!!!!!!
                for(int i = res.Count - 1; i >= 0; i--)
                {
                    if (res[i].Dep == 270)
                    {
                        if (res[i].Summ == 0 && res[i].Count == 0)
                            res.RemoveAt(i);
                        else
                            res[i].Dep = 302;
                    }
                    if (res[i].Dep == 380)
                    {
                        if (res[i].Summ == 0 && res[i].Count == 0)
                            res.RemoveAt(i);
                        else
                            res[i].Dep = 301;
                    }
                }

                //              !!!!!!!!!!!!!!!!!!!!!! жуткий костыль, как и сам факт наличия значений на уч 111
                db.ReportDayResult.InsertAllOnSubmit(res.Where(_res => _res.Dep != 111));// && _res.Dep != 270 && _res.Dep != 380));
                db.SubmitChanges();
            }
        }

        public void RecalcMonth(DateTime month)
        {
            var db = new ReportBaseDataContext();
            var resDay = db.ReportDayResult.Where(_res => _res.TypeId == TypeId && _res.BD.Value.Year == month.Year && _res.BD.Value.Month == month.Month).ToList();
            List<ReportMonthResult> resMonth = new List<ReportMonthResult>();
            var Serv = new S2010.XrepSoapClient();
            var DepList = Serv.GetPointList3();
            var resOut = new List<ReportDayResult>();
            foreach (S2010.DepartmentInfo Dii in DepList.Where(_dep => _dep.Enabled))
            {
                var resByDep = resDay.Where(_res => _res.Dep == Dii.Number);
                double? totalSumm = (double?)resByDep.Sum(s => s.Summ);
                double? totalCount = (double?)resByDep.Sum(c => c.Count);
                if (totalCount != null && totalCount > 0)
                {
                    double value = (double)((totalSumm != null ? totalSumm : 0) / totalCount);
                    resMonth.Add(new ReportMonthResult()
                    {
                        Department = Dii.Number,
                        DepName = Dii.Name,
                        Month = month,
                        Type = TypeId,
                        Value = totalSumm,
                        Value2 = totalCount,
                        Value3 = value
                    });
                }
            }

            if (db.ReportMonthResults.Any(a => a.Month == month && a.Type == TypeId))
            {
                var del = db.ReportMonthResults.Where(a => a.Month == month && a.Type == TypeId);
                db.ReportMonthResults.DeleteAllOnSubmit(del);
                db.SubmitChanges();
            }
            if (resMonth.Count() > 0)
            {
                db.ReportMonthResults.InsertAllOnSubmit(resMonth);
                db.SubmitChanges();
            }
        }
        protected ReportDayResult GetReportDayResult(S2010.DepartmentInfo Dii=null)
        {
            var res = new ReportDayResult()
            {
                TypeId = this.TypeId
            };
            if (Dii != null)
            {
                res.Dep = Dii.Number;
                res.DepName = Dii.Name;
            }
            return res;
        }
    }
}
