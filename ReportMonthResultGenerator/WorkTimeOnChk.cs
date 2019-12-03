using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
    class WorkTimeOnChk
    {
        public static List<ReportMonthResult> GetWorkTimeOnChk(DateTime dt1)
        {
            DateTime dt2 = dt1.AddMonths(1).AddDays(-1);
            List<ReportMonthResult> Tmp = new List<ReportMonthResult>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            StaffEmpl.StaffEmployeeParametersObjClient cl = new StaffEmpl.StaffEmployeeParametersObjClient();
            Dictionary<int, int> ChecksCount = сRashMat.GetChkCount(dt1);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                try
                {
                    decimal? H = 0;
                    cl.WORKHOUR(dt1, dt2.AddDays(-1), Dii.Number, out H);
                    Console.WriteLine("WorkTime {0} - {1}", Dii.Name, H);
                    ReportMonthResult RM = new ReportMonthResult()
                    {

                        Department = Dii.Number,
                        Value = (double)H,
                        Value2 = ChecksCount[Dii.Number],
                        Value3 = (double)H / ChecksCount[Dii.Number]

                    };
                    Tmp.Add(RM);
                }
                catch
                { }
            }
            return Tmp;
        }
    }
}
