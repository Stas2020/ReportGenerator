using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    public class ProductivCook : ProductivityBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            if (!Productivity.HasGoodsCalculatedForDay(day))
                Productivity.CalculateGoodsForDay(day);

            return Productivity.CalcForDay(day, this);
        }

    }
}