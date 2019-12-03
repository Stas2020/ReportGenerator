using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class WTSCalc:CalcBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            var resOut = new List<ReportDayResult>();
            var wts = StaffWtToExcel.GetWtsNoWaiterByDay(day);

            return resOut;
        }
        }
    }
