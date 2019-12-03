using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class ShiftSpeesCalc : CalcBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            double limitSecs = 300;
            var resOut = new List<ReportDayResult>();

            string tmpPath = @"tmp\";
            string dirPath = @"\\nas\winshare\DigitalFolders\";
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово").OrderBy(a => a.Name))
            {
                string fPath = $@"{dirPath}{Dii.Number.ToString()}\{day.ToString("yyyyMMdd")}.zip";
                string outPath = $@"{tmpPath}{day.ToString("yyyyMMdd")}";
                try
                {


                    Utils.ToLog($"ShiftSpeesCalc {fPath}", true);
                    if (!System.IO.Directory.Exists(tmpPath))
                    {
                        System.IO.Directory.CreateDirectory(tmpPath);
                    }
                    if (System.IO.Directory.Exists(outPath))
                    {
                        System.IO.Directory.Delete(outPath, true);
                    }
                    if (!System.IO.File.Exists(fPath))
                    {
                        Utils.ToLog($"Not exists path", true);
                        continue;
                    }
                    System.IO.Compression.ZipFile.ExtractToDirectory(fPath, outPath);
                    DbfData.DBFSpeedOfQueue sos = new DbfData.DBFSpeedOfQueue(outPath);
                    sos.Parse(outPath);
                    System.IO.Directory.Delete(outPath, true);
                    var chks = AirReports.GetQSChecks(Dii.Number, day, day.AddDays(1));
                    int chCount = chks.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Count();
                    double tCount = chks.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Sum(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds);
                    ReportDayResult res = GetReportDayResult(Dii);
                    res.BD = day;
                    res.Count = chCount;
                    res.Summ = tCount;
                    res.Value = chCount != 0 ? tCount / chCount : 0;
                    resOut.Add(res);
                }
                catch (Exception e)
                {
                    Utils.ToLog($"Error ShiftSpeesCalc {fPath} {e.Message}", true);
                }
            }

            return resOut;
        }
        /*
        private void UnZip(s)
        {

        }
        */
    }
}
