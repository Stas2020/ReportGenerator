using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class DishesCalc : CalcBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            //    var res = Spisanie.GetDesertsSpis(day, day.AddDays(1));

            var DishesList = CubeData.GetDishOfCat(41); //Блюда кухни
            DishesList.AddRange(CubeData.GetDishOfCat(54));// десерты
            var DishesCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DishesList.ToArray()),Day:day.Day));

            var Serv = new S2010.XrepSoapClient();
            var DepList = Serv.GetPointList3();
            Dictionary<int, int> ChecksCount = CheckCountSingletone.Instance.RestNonZeroOnlyCheckCount.GetCheckCount(day);
            var resOut = new List<ReportDayResult>();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (ChecksCount.TryGetValue(Dii.Number, out int chCount))
                {
                    var DCount = DishesCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                    
                    var res = new ReportDayResult()
                    {
                        BD = day,
                        TypeId = TypeId,
                        Dep = Dii.Number,
                        DepName = Dii.Name,
                        Count = chCount,
                        Summ = (double)(DCount),
                        Value = chCount != 0 ? (double)((DCount) / chCount) : 0
                    };
                    resOut.Add(res);
                }
            }
            return resOut;
        }

    }
}
