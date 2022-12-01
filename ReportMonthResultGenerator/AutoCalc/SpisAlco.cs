using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class SpisAlco : SpisanieBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            CalcType = SpisanieType.Alco;
            return base.Calc(day);
        }
    }
}