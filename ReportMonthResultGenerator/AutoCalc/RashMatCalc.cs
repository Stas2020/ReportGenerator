using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class RashMatCalc: CalcBase
    {
        public RashMatCalc()
        { }

        public override List<ReportDayResult> Calc(DateTime day)
        {
            var res = CorrectTwinDep.Correct(сRashMat.GetRashMatByDay(day, day.AddDays(1)));
            List<ReportDayResult> resOut = res.Select(a => new ReportDayResult()
            {
                BD = day,
                Count = a.Checks,
                Dep = a.Dep,
                DepName = a.DepName,
                Summ = a.Value,
                Value = a.ValueOnCheck,
                TypeId = TypeId
            }).ToList();
            return resOut;
        }
    }
}
