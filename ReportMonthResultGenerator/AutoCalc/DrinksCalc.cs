using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class DrinksCalc : CalcBase
    {
        //////// Хардкод - диапазон столов, ToDo вынести в админку
        //////public static List<TimeOfPreparation.Range> excludeTables = new List<TimeOfPreparation.Range>() { new TimeOfPreparation.Range(144, 255), new TimeOfPreparation.Range(900, 999) };
        public override List<ReportDayResult> Calc(DateTime day)
        {
            var Serv = new S2010.XrepSoapClient();
            var DepList = Serv.GetPointList3();
            var resOut = new List<ReportDayResult>();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                var data = MOZGIntegration.GetRestData(day, Dii.Number);
                if(data != null)
                {
                    var res = new ReportDayResult()
                    {
                        BD = day,
                        TypeId = TypeId,
                        Dep = Dii.Number,
                        DepName = Dii.Name,
                        Count = data.ChecksCount,
                        Summ = (double)(data.DrinksCount),
                        Value = data.ChecksCount != 0 ? (double)((data.DrinksCount) / data.ChecksCount) : 0
                    };
                    resOut.Add(res);
                }
            }
            return resOut;



            //// /////////////// IT's OLD ///////////////////////

            ////    var res = Spisanie.GetDesertsSpis(day, day.AddDays(1));


            ////////////Было так
            ////var DrinksListCup = CubeData.GetDishOfCat(92); //напитки в бокалах
            ////var DrinksCupCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DrinksListCup.ToArray()), Day: day.Day));

            //////Кол - во напитков в бутылках
            ////var DrinksListBottle = CubeData.GetDishOfCat(91); //напитки в бутылках
            ////var DrinksBottleCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DrinksListBottle.ToArray()), Day: day.Day));

            ////var bottleCoef = 4;




            //////// Так по-новому
            ////var DrinksListBottle = CubeData.GetDishOfCat(91); //напитки в бутылках
            ////////var DrinksListCup = CubeData.GetDishOfCat(91); //напитки в бокалах

            ////var drinksCats = new List<int>() { 1, 3, 38, 4, 5, 7, 17, 19, 20, 21, 22, 32, 60 }; //  напитки
            //var drinksCats = new List<int>() { 1, 3, 4, 5, 7, 17, 19, 20, 21, 22, 26, 32, 38, /**/28,/**/ }; //  напитки
            //var cat28dishes = new List<int>() { 842222, 2820, 847214, 844323, 862195, 845089, 842242 };

            ////var DrinksCupCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DrinksListBottle.ToArray()), Day: day.Day, OnlyCats: drinksCats, ExcludeBarCodes: true, UseDishesCoeff: true));
            ////var DrinksBottleCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DrinksListBottle.ToArray()), Day: day.Day, OnlyCats: drinksCats, ExcludeBarCodes: false, UseDishesCoeff: true));
            ////var DrinksCommonCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, "0", Day: day.Day, OnlyCats: drinksCats, ExcludeBarCodes: true, UseDishesCoeff: true));
            //var DrinksCommonCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", cat28dishes), Day: day.Day, OnlyCats: drinksCats, ExcludeBarCodes: true, UseDishesCoeff: true));

            ////var bottleCoef = 1;// теперь расчет через таблицу [Diogen].[dbo].[DishCoeffsKissTheCook] // 6;// 4







            //var Serv = new S2010.XrepSoapClient();
            //var DepList = Serv.GetPointList3();
            //Dictionary<int, int> ChecksCount = CheckCountSingletone.Instance.RestNonZeroOnlyCheckCount.GetCheckCount(day);
            //var resOut = new List<ReportDayResult>();
            //foreach (S2010.DepartmentInfo Dii in DepList)
            //{
            //    if (ChecksCount.TryGetValue(Dii.Number, out int chCount))
            //    {
            //        //var drcupCount = DrinksCupCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
            //        //var drbottleCount = DrinksBottleCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
            //        var drCommonCount = DrinksCommonCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
            //        var res = new ReportDayResult()
            //        {
            //            BD = day,
            //            TypeId = TypeId,
            //            Dep = Dii.Number,
            //            DepName = Dii.Name,
            //            Count = chCount,
            //            Summ = (double)(drCommonCount),
            //            Value = chCount != 0 ? (double)((drCommonCount) / chCount) : 0
            //            //Summ = (double)(drcupCount + drbottleCount /* * bottleCoef*/),
            //            //Value = chCount != 0 ? (double)((drcupCount + drbottleCount /* * bottleCoef*/) / chCount) : 0
            //        };
            //        resOut.Add(res);
            //    }
            //}
            //return resOut;
        }

    }
}
