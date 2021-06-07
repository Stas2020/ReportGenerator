using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class OrderTimeWODeliveryCalc : CalcBase
    {

        public override List<ReportDayResult> Calc(DateTime day)
        {
            var res = TimeOfPreparation.GetTimeOfPrepOrder(day, day.AddDays(1), true);
            List<ReportDayResult> resOut = res.Select(a => new ReportDayResult()
            {
                BD = day,
                Count = a.AllCount,
                Dep = a.Dep,
                DepName = a.DepName,
                Summ = a.WrongCount,
                Value = a.Percent,
                TypeId = TypeId
            }).ToList();
            return resOut;
        }
    }
}
