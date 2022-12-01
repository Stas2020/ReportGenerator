using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class SpisDishes : SpisanieBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            CalcType = SpisanieType.Dishes;
            return base.Calc(day);
        }
    }
}
