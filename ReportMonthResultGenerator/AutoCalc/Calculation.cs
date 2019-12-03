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
        public static void Init()
        {
            //calculationsList.Add(new AutoCalc.ShiftSpeesCalc());
            //calculationsList.Add(new AutoCalc.DrinksCalc());
            //calculationsList.Add(new AutoCalc.RashMatCalc());
            //calculationsList.Add(new AutoCalc.DesertSpisCalc());

            
            calculationsList.Add(new AutoCalc.ShiftSpeesCalc());

            calculationsList.Add(new AutoCalc.OrderTimeCalc());
            
            calculationsList.Add(new AutoCalc.DesertSpisCalc());
            
            calculationsList.Add(new AutoCalc.RashMatCalc());
            
            calculationsList.Add(new AutoCalc.DrinksCalc());
            calculationsList.Add(new AutoCalc.DishesCalc());
            


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
    }

    public abstract class CalcBase
    {
        public CalcBase()
        { }

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

        public abstract List<ReportDayResult> Calc(DateTime day);

        public void InsertData(DateTime day)
        {
            var res = Calc(day);
            if (res != null)
            {
                var db = new ReportBaseDataContext();
                if (db.ReportDayResult.Any(a => a.BD == day && a.TypeId==TypeId))
                {
                    var del = db.ReportDayResult.Where(a => a.BD == day && a.TypeId == TypeId);
                    db.ReportDayResult.DeleteAllOnSubmit(del);
                    db.SubmitChanges();
                }
                db.ReportDayResult.InsertAllOnSubmit(res);
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
