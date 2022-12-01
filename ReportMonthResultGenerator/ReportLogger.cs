using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;



namespace ReportMonthResultGenerator
{
   static class ReportLogger
    {
       static DateTime? startDate;
       static int id;

        public static void InitDay(DateTime _day/*, bool _clearDay = false*/)
        {            
            startDate = DateTime.Now;
            using (var db = new ReportBaseDataContext())
            {
                //if(_clearDay)
                //{
                //    var del = db.ReportLog.Where(a => a.Day == _day);

                //    db.ReportLog.DeleteAllOnSubmit(del);
                //    db.SubmitChanges();
                //}
                var del = db.ReportLog.Where(a => a.Result > 0);
                db.ReportLog.DeleteAllOnSubmit(del);
                db.SubmitChanges();

                var rec = new ReportLog()
                {
                    Day = _day,
                    DateStart = (DateTime)startDate
                };
                db.ReportLog.InsertAllOnSubmit(new List<ReportLog>() { rec });
                db.SubmitChanges();
                id = rec.Id;
            }
        }
        public static void UpdateDay(DateTime _day, int _type)
        {
            if (startDate == null)
                return;
            using (var db = new ReportBaseDataContext())
            {
                var rec = db.ReportLog.FirstOrDefault(_rec => _rec.Day == _day && _rec.Id == id);
                if(rec != null)
                {
                    rec.Result = _type;
                    db.SubmitChanges();
                }
            }
        }
        public static void MarkErrorDay(DateTime _day)
        {
            if (startDate == null)
                return;
            using (var db = new ReportBaseDataContext())
            {
                var rec = db.ReportLog.FirstOrDefault(_rec => _rec.Day == _day && _rec.Id == id);
                if (rec != null)
                {
                    rec.Result = -Math.Abs((int)rec.Result);
                    rec.DateFinish = DateTime.Now;
                    db.SubmitChanges();
                }
            }
        }
        public static void CloseDayOk(DateTime _day)
        {
            if (startDate == null)
                return;

            using (var db = new ReportBaseDataContext())
            {
                var rec = db.ReportLog.FirstOrDefault(_rec => _rec.Day == _day && _rec.Id == id);
                if (rec != null)
                {
                    rec.DateFinish = DateTime.Now;
                    rec.Result = 0;
                    db.SubmitChanges();
                }
            }
        }        
    }
   
}
