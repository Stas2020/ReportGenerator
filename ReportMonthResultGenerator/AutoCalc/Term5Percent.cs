using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class Term5Percent : CalcBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            var resOut = new List<ReportDayResult>();

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            var db = new ReportBaseDataContext();

            foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово").OrderBy(a => a.Name))
            {
                try
                {
                    var records = db.CheckSummByTerm.Where(_rec => _rec.BusinessDate == day && _rec.Dep == Dii.Number);//.ToList();

                    var summAll = records.Sum(_rec => _rec.RealSumm);
                    var summ5Term = records.Where(_rec => _rec.TerminalId == 5).Sum(_rec => _rec.RealSumm);

                    ReportDayResult res = GetReportDayResult(Dii);
                    res.BD = day;
                    res.Count = summAll;
                    res.Summ = summ5Term;
                    res.Value = summAll != 0 ? summ5Term / summAll : 0;
                    resOut.Add(res);
                }
                catch (Exception e)
                {
                    Utils.ToLog($"Error Term5Percent {e.Message}", true);
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
