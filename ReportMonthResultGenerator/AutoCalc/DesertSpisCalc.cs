using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class DesertSpisCalc : CalcBase
    {
     
        public override List<ReportDayResult> Calc(DateTime day)
        {
            var res = Spisanie.GetDesertsSpis(day, day.AddDays(1));
            List<ReportDayResult> resOut = res.Select(a => new ReportDayResult()
            {
                BD = day,
                Count = a.Value,
                Dep = a.Department,
                DepName = a.DepName,
                Summ = a.Value2,
                Value = a.Value3,
                TypeId = TypeId
            }).ToList();
            return resOut;
        }
    }
}
