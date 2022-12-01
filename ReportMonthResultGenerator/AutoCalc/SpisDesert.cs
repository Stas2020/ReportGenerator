using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class SpisDesert: SpisanieBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            CalcType = SpisanieType.Desert;
            return base.Calc(day);
        }
    }
}