using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
    static class ReportTableUpdater
    {
        static ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");




        public static void OrderTimeBySuSheff(List<ReportDayQSRTime> Data , DateTime StartDate,  DateTime EndDate)
        {
            IQueryable<ReportDayQSRTime> res = from o in RepBase.ReportDayQSRTimes where (o.Day >= StartDate && o.Day<EndDate) select o;

            if (res.Count() > 0)
            {
                RepBase.ReportDayQSRTimes.DeleteAllOnSubmit(res);
                RepBase.SubmitChanges();
            }

            RepBase.ReportDayQSRTimes.InsertAllOnSubmit(Data);
            RepBase.SubmitChanges();



        }

        public static void OrderTimeBySuSheffOrders(List<ReportDayQSRTimeByOrders> Data, DateTime StartDate, DateTime EndDate)
        {
            IQueryable<ReportDayQSRTimeByOrders> res = from o in RepBase.ReportDayQSRTimeByOrders where (o.Day >= StartDate && o.Day < EndDate) select o;

            if (res.Count() > 0)
            {
                RepBase.ReportDayQSRTimeByOrders.DeleteAllOnSubmit(res);
                RepBase.SubmitChanges();
            }

            RepBase.ReportDayQSRTimeByOrders.InsertAllOnSubmit(Data);
            RepBase.SubmitChanges();



        }
        public static void DismPercentUpdate(List<StaffDismissal> Dp, DateTime Month)
        {


            foreach (StaffDismissal Sd in Dp)
            {

                IQueryable<ReportMonthResult> res = from o in RepBase.ReportMonthResults where (o.Month == Month && o.Department == Sd.Dep && o.Type == 1) select o;

                if (res.Count() >0)
                {
                    RepBase.ReportMonthResults.DeleteAllOnSubmit(res);
                    RepBase.SubmitChanges();
                }
                ReportMonthResult RmR = new ReportMonthResult()
                {
                    Department = Sd.Dep,
                    Month = Month,
                    Type = 1,
                    Value = Sd.Percent,
                    Value2 = Sd.StaffCount,
                    Value3 = Sd.DismissCount,
                    DepName = Sd.DepName

                };
                RepBase.ReportMonthResults.InsertOnSubmit(RmR);
            }
            RepBase.SubmitChanges();

        }
        public static List<ReportMonthResult> GetDesertOnStop(DateTime Month)
        {
            
            List<ReportMonthResult> res = (from o in RepBase.ReportMonthResults where (o.Month == Month && o.Type == 4) select o).ToList();
            //foreach ()
            return res;
        }
        public static List<ReportMonthResult> GetDesertOnChk(DateTime Month)
        {

            List<ReportMonthResult> res = (from o in RepBase.ReportMonthResults where (o.Month == Month && o.Type == 6) select o).ToList();
            //foreach ()
            return res;
        }
        public static List<ReportMonthResult> GetDesertSpisanie(DateTime Month)
        {

            List<ReportMonthResult> res = (from o in RepBase.ReportMonthResults where (o.Month == Month && o.Type == 5) select o).ToList();
            //foreach ()
            return res;
        }

        public static void DesertOnCheckUpdate(List<ReportMonthResult> Dp, DateTime Month)
        {
            foreach (ReportMonthResult Sd in Dp)
            {
                IQueryable<ReportMonthResult> res = from o in RepBase.ReportMonthResults where (o.Month == Month && o.Department == Sd.Department && o.Type == 6) select o;

                if (res.Count() > 0)
                {
                    RepBase.ReportMonthResults.DeleteAllOnSubmit(res);
                    RepBase.SubmitChanges();
                }
                /*
                ReportMonthResult RmR = new ReportMonthResult()
                {
                    Department = Sd.Dep,
                    Month = Month,
                    Type = 4,
                    Value = Sd.MinOnStop,
                    //Value2 = Sd.StaffCount,
                    //Value3 = Sd.DismissCount,
                    DepName = Sd.DepName

                };
                 * */
                Sd.Month = Month;
                Sd.Type = 6;
                Sd.Value3 = Sd.Value2 / Sd.Value;
                RepBase.ReportMonthResults.InsertOnSubmit(Sd);
            }
            RepBase.SubmitChanges();

        }

        public static void DesertSpisanieUpdate(List<ReportMonthResult> Dp, DateTime Month)
        {
            foreach (ReportMonthResult Sd in Dp)
            {
                IQueryable<ReportMonthResult> res = from o in RepBase.ReportMonthResults where (o.Month == Month && o.Department == Sd.Department && o.Type == 5) select o;

                if (res.Count() > 0)
                {
                    RepBase.ReportMonthResults.DeleteAllOnSubmit(res);
                    RepBase.SubmitChanges();
                }
                /*
                ReportMonthResult RmR = new ReportMonthResult()
                {
                    Department = Sd.Dep,
                    Month = Month,
                    Type = 4,
                    Value = Sd.MinOnStop,
                    //Value2 = Sd.StaffCount,
                    //Value3 = Sd.DismissCount,
                    DepName = Sd.DepName

                };
                 * */
                Sd.Month = Month;
                Sd.Type = 5;
                RepBase.ReportMonthResults.InsertOnSubmit(Sd);
            }
            RepBase.SubmitChanges();

        }

        public static void DesertOnStopUpdate(List<DesertsOnStopResult> Dp, DateTime Month)
        {
            foreach (DesertsOnStopResult Sd in Dp)
            {
                IQueryable<ReportMonthResult> res = from o in RepBase.ReportMonthResults where (o.Month == Month && o.Department == Sd.Dep && o.Type == 4) select o;

                if (res.Count() > 0)
                {
                    RepBase.ReportMonthResults.DeleteAllOnSubmit(res);
                    RepBase.SubmitChanges();
                }
                ReportMonthResult RmR = new ReportMonthResult()
                {
                    Department = Sd.Dep,
                    Month = Month,
                    Type = 4,
                    Value = Sd.MinOnStop ,
                    //Value2 = Sd.StaffCount,
                    //Value3 = Sd.DismissCount,
                    DepName = Sd.DepName

                };
                RepBase.ReportMonthResults.InsertOnSubmit(RmR);
            }
            RepBase.SubmitChanges();

        }
        public static List<ReportMonthResult> GetOrderTime(DateTime Month)
        {
            List<RashMaterials> Tmp = new List<RashMaterials>();
            List<ReportMonthResult> res = (from o in RepBase.ReportMonthResults where (o.Month == Month && o.Type == 2) select o).ToList();
            //foreach ()
            return res;
        }

        public static void OrderTimeUpdate(List<PrepTime> Dp, DateTime Month)
        {


            foreach (PrepTime Sd in Dp)
            {

                IQueryable<ReportMonthResult> res = from o in RepBase.ReportMonthResults where (o.Month == Month && o.Department == Sd.Dep&& o.Type==2) select o;

                if (res.Count() > 0)
                {
                    RepBase.ReportMonthResults.DeleteAllOnSubmit(res);
                    RepBase.SubmitChanges();
                }
                ReportMonthResult RmR = new ReportMonthResult()
                {
                    Department = Sd.Dep,
                    Month = Month,
                    Type = 2,
                    Value = Sd.WrongSecond,
                    Value2 = Sd.AllCount,
                    Value3 = Sd.WrongCount,
                    DepName = Sd.DepName
                };
                RepBase.ReportMonthResults.InsertOnSubmit(RmR);
            }
            RepBase.SubmitChanges();

        }
        public static List<ReportMonthResult> GetRashMaterials(DateTime Month)
        {
            List<RashMaterials> Tmp = new List<RashMaterials>();
            List<ReportMonthResult> res = (from o in RepBase.ReportMonthResults where (o.Month == Month && o.Type == 3) select o).ToList();
            //foreach ()
            return res;
        }
        public static void RashMatUpdate(List<RashMaterials> Dp, DateTime Month)
        {


            foreach (RashMaterials Sd in Dp)
            {

                IQueryable<ReportMonthResult> res = from o in RepBase.ReportMonthResults where (o.Month == Month && o.Department == Sd.Dep && o.Type == 3) select o;

                if (res.Count() > 0)
                {
                    RepBase.ReportMonthResults.DeleteAllOnSubmit(res);
                    RepBase.SubmitChanges();
                }
                ReportMonthResult RmR = new ReportMonthResult()
                {
                    Department = Sd.Dep,
                    Month = Month,
                    Type = 3,
                    Value = Sd.Value,
                    Value2 = Sd.Checks,
                    Value3 = Sd.ValueOnCheck,
                    DepName = Sd.DepName
                };
                RepBase.ReportMonthResults.InsertOnSubmit(RmR);
            }
            RepBase.SubmitChanges();

        }

    }
}
