using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class SpisDrinks : SpisanieBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            CalcType = SpisanieType.Drinks;
            return base.Calc(day);
        }
    }
}
