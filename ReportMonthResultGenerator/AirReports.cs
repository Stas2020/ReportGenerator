using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator
{

    public class WTOnCheck
    {
        public int ChkCount { set; get; }
        public double WtCount { set; get; }
        public double HoursOnCheck
        {
            get
            {
                return ChkCount != 0 ? (double)WtCount / (double)ChkCount : 0;
            }
        }
    }

    class AirReports
    {
        public static Dictionary<int, WTOnCheck> GetWTOnCheck(DateTime Month)
        {
            var res = new Dictionary<int, WTOnCheck>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            var chkCount = CubeData.GetChecksCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово"))
            {
                if (!Dii.Enabled) continue;

                for (var dt = new DateTime(2018, 12, 1); dt <= new DateTime(2018, 12, 31); dt = dt.AddDays(1))
                {
                    string DBPath = $@"\\cube2005\g$\ArhivFilesDownload\data\DBF\{Dii.AlohaStr}1\{dt.ToString("yyyy/MM/dd")}";
                    //Парсим

                }

                List<CEmplWt> Wts = StaffBase.GetWtsByDep(Dii.Number, Month, Month.AddMonths(1));
                double wtsCount = Wts.Sum(a => (StaffWtToExcel.GetMaxDate(a.StopDt, Month) - StaffWtToExcel.GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                int mchkCount = 0;
                chkCount.TryGetValue(Dii.Number, out mchkCount);

                res.Add(Dii.Number, new WTOnCheck()
                {
                    ChkCount = mchkCount,
                    WtCount = wtsCount
                });
                /*
                if (mchkCount != 0)
                {
                    res.Add(Dii.Number, wtsCount / (double)mchkCount);
                }
                */

            }
            return res;
        }

        public static void GetWTOnCheckTest(DateTime Month)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;



            Cube2005DataContext db = new Cube2005DataContext();
            db.CommandTimeout = 0;
            //  db.Connection.ConnectionTimeout = 0;
            int rowNum = 2;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            //var chkCount = CubeData.GetChecksCount(Month);
            var Wts = GetWTOnCheck(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово").OrderBy(a => a.Name))
            {
                WTOnCheck w = new WTOnCheck();
                if (Wts.TryGetValue(Dii.Number, out w))
                {
                    Ws.Cells[rowNum, 1] = Dii.Name;
                    Ws.Cells[rowNum, 2] = w.WtCount;
                    Ws.Cells[rowNum, 3] = w.ChkCount;
                }
                rowNum++;

            }
        }




      

        public static Dictionary<int, decimal> GetSpeedOfServAvg(DateTime Month)
        {
            var res = new Dictionary<int, decimal>();
            Cube2005DataContext db = new Cube2005DataContext();
            db.CommandTimeout = 0;
            //  db.Connection.ConnectionTimeout = 0;
            int rowNum = 2;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            var chkCount = CubeData.GetChecksCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово").OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                rowNum++;

                var chks = GetQSChecks(Dii.Number, Month, Month.AddMonths(1));
                int chCount = chks.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < 300 && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Count();
                double tCount = chks.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < 300 && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Sum(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds);
                if (chCount > 0)
                {
                    res.Add(Dii.Number, (decimal)tCount / (decimal)chCount);
                }


            }
            return res;
        }


        public static void GetSpeedOfServTest(DateTime Month)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            Ws.Cells[1, 1] = "Пластик";
            for (int dc = 1; dc < 11; dc++)
            {
                Ws.Cells[2, (dc - 1) * 2 + 2] = "Блюд:" + dc;

            }
            Cube2005DataContext db = new Cube2005DataContext();
            db.CommandTimeout = 0;
            //  db.Connection.ConnectionTimeout = 0;
            int rowNum = 2;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            var chkCount = CubeData.GetChecksCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово").OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                rowNum++;
                Ws.Cells[rowNum, 1] = Dii.Name;

                var chks = GetQSChecks(Dii.Number, Month, Month.AddMonths(1));

                int AllchCount = 0;
                double AlltCount = 0;

                if (chks.Count() == 0) continue;
                for (int dc = 1; dc < 11; dc++)
                {
                    int chCount = chks.Where(a => a.QSDishes.Count() == dc && (a.TClose - a.FirstDishOpenTime).TotalSeconds < 600 && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Count();
                    double tCount = chks.Where(a => a.QSDishes.Count() == dc && (a.TClose - a.FirstDishOpenTime).TotalSeconds < 600 && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Sum(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds);
                    Ws.Cells[rowNum, (dc - 1) * 2 + 2] = (tCount / (double)chCount).ToString("0.00");
                    Ws.Cells[rowNum, (dc - 1) * 2 + 3] = chCount;
                    AllchCount += chCount;
                    AlltCount += tCount;
                }
                Ws.Cells[rowNum, (12 - 1) * 2 + 2] = (AlltCount / (double)AllchCount).ToString("0.00");
                Ws.Cells[rowNum, (12 - 1) * 2 + 3] = AllchCount;
                /*
                int dnum = Dii.Number;
                var chks = db.XmlChecks.Where(a => a.Dep == dnum && a.SystemDate >= Month && a.SystemDate < Month.AddMonths(1));
                chks = chks.Where(a => a.SystemDate.HasValue && a.SystemDateOfOpen.HasValue);
                chks = chks.Where(a => a.Card > 0);
                if (chks.Count() == 0) continue;
                for (int dc = 1; dc < 11; dc++)
                {
                    int chCount = chks.Where(a => a.XmlChecksDishes.Count() == dc).Count();
                    double tCount = chks.Where(a => a.XmlChecksDishes.Count() == dc).Sum(a => (a.SystemDate.Value - a.SystemDateOfOpen.Value).TotalSeconds);
                    Ws.Cells[rowNum, (dc - 1) * 2 + 2] = (tCount / (double)chCount).ToString("0.00");
                    Ws.Cells[rowNum, (dc - 1) * 2 + 3] = chCount;

                }
                */
            }
        }

        public static List<QSCheck> GetQSChecks(int dep, DateTime StartDt, DateTime StopDt)
        {
            List<QSCheck> Tmp = new List<QSCheck>();
            var db = new CheckLongTestDbDataContext();
            var checks = db.Turns.Where(a => a.DepNum == dep && a.OpenTime >= StartDt && a.CloseTime < StopDt);
            var items = db.Items.Where(a => a.DepNum == dep && a.StartTime >= StartDt && a.StartTime < StopDt).ToList();
            foreach (var chk in checks)
            {
                var DList = new List<QSDish>();

                IEnumerable<DateTime?> dd;
                if (chk.CheckNum.HasValue)
                {

                    dd = items.Where(a => a.CheckNum == chk.CheckNum && a.DepNum == chk.DepNum && a.DOB == chk.DOB).Select(a => a.StartTime);
                }
                else
                {
                    dd = items.Where(a => a.TableId == chk.TableId && a.DepNum == chk.DepNum && a.DOB == chk.DOB).Select(a => a.StartTime);

                }

                foreach (var d in dd)
                {
                    DList.Add(new QSDish() { TAdd = d.Value });
                }

                Tmp.Add(new QSCheck()
                {
                    TableNum = chk.TableId.GetValueOrDefault(0),
                    CheckNum = chk.CheckNum.GetValueOrDefault(0),
                    DB = chk.DOB.Value,
                    Dep = chk.DepNum.Value,
                    TClose = chk.CloseTime.Value,
                    TOpen = chk.OpenTime.Value,
                    QSDishes = DList
                });

            }
            return Tmp;

        }
    }


    public class QSCheck
    {
        public QSCheck()
        { }
        public DateTime TClose;
        public DateTime TOpen;
        public int Dep;
        public DateTime DB;
        public int CheckNum;
        public int TableNum;
        public List<QSDish> QSDishes;

        public DateTime FirstDishOpenTime
        {
            get
            {
                if ((QSDishes == null) || (QSDishes.Count == 0)) { return TOpen; }
                var res = QSDishes.Min(a => a.TAdd);
                if (res > TClose)
                {
                    return res.AddDays(-1);
                }
                return res;

            }
        }
    }

    public class QSDish
    {
        public DateTime TAdd;
    }
}