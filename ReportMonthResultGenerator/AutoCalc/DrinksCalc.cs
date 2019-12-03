using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class DrinksCalc : CalcBase
    {
        public override List<ReportDayResult> Calc(DateTime day)
        {
            //    var res = Spisanie.GetDesertsSpis(day, day.AddDays(1));

            var DrinksListCup = CubeData.GetDishOfCat(92); //напитки в бокалах
            var DrinksCupCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DrinksListCup.ToArray()), Day: day.Day));

            //Кол-во напитков в бутылках
            var DrinksListBottle = CubeData.GetDishOfCat(91); //напитки в бутылках
            var DrinksBottleCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DrinksListBottle.ToArray()), Day: day.Day));

            var Serv = new S2010.XrepSoapClient();
            var DepList = Serv.GetPointList3();
            Dictionary<int, int> ChecksCount = CheckCountSingletone.Instance.RestNonZeroOnlyCheckCount.GetCheckCount(day);
            var resOut = new List<ReportDayResult>();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (ChecksCount.TryGetValue(Dii.Number, out int chCount))
                {
                    var drcupCount = DrinksCupCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                    var drbottleCount = DrinksBottleCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                    var res = new ReportDayResult()
                    {
                        BD = day,
                        TypeId = TypeId,
                        Dep = Dii.Number,
                        DepName = Dii.Name,
                        Count = chCount,
                        Summ = (double)(drcupCount + drbottleCount * 4),
                        Value = chCount != 0 ? (double)((drcupCount + drbottleCount * 4) / chCount) : 0
                    };
                    resOut.Add(res);
                }
            }
            return resOut;
        }

    }
}
