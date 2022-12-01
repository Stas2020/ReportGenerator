using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class DishesCalc : CalcBase
    {
        // Хардкод - диапазон столов, ToDo вынести в админку
        // nтак было до 04.02.2022
        //public static List<TimeOfPreparation.Range> excludeTables = new List<TimeOfPreparation.Range>() { new TimeOfPreparation.Range(144, 255), new TimeOfPreparation.Range(900, 999) };
        public static List<TimeOfPreparation.Range> excludeTables = new List<TimeOfPreparation.Range>() { new TimeOfPreparation.Range(144, 300), new TimeOfPreparation.Range(800, 999) };
        public static double minCorrectValue = 0.15;// Меньше 0,1 напитка/блюда на гостя продаваться не может, поэтому такие значения не учитывать
        public override List<ReportDayResult> Calc(DateTime day)
        {
            var Serv = new S2010.XrepSoapClient();
            var DepList = Serv.GetPointList3();
            var resOut = new List<ReportDayResult>();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                var data = MOZGIntegration.GetRestData(day, Dii.Number);
                if (data != null)
                {
                    var res = new ReportDayResult()
                    {
                        BD = day,
                        TypeId = TypeId,
                        Dep = Dii.Number,
                        DepName = Dii.Name,
                        Count = data.ChecksCount,
                        Summ = (double)(data.DishesCount),
                        Value = data.ChecksCount != 0 ? (double)((data.DishesCount) / data.ChecksCount) : 0
                    };
                    resOut.Add(res);
                }
            }
            return resOut;



            //// /////////////// IT's OLD ///////////////////////
            //    var res = Spisanie.GetDesertsSpis(day, day.AddDays(1));


            //////// Было так
            ////var DishesList = CubeData.GetDishOfCat(41); //Блюда кухни
            ////DishesList.AddRange(CubeData.GetDishOfCat(54));// десерты

            ////var DishesCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", DishesList.ToArray()), Day: day.Day));

            //////// Так по-новому  -6 -16 -23 -27  
            ////var dishesCats = new List<int>() { 8, 9, 10, 11, 13, 15, 16, 18, 23, 26, 27, 28, 29, 30, 31, 33, 34, 35 }; //  еда и десерты
            //var dishesCats = new List<int>() { 6, 8, 9, 10, 11, 13, 14, 15, 16, 18, 23, 27, /**/28,/**/ 29, 30, 31, 33, 35 }; //  еда и десерты
            //var cat28drinks = new List<int>() { 330, 882214, 825055 };

            ////var DishesCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, "", AllCodes: true, Day: day.Day, OnlyCats: dishesCats, ExcludeBarCodes: false, UseDishesCoeff: true));
            //var DishesCount = CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(day, string.Join(",", cat28drinks), Day: day.Day, OnlyCats: dishesCats, ExcludeBarCodes: true, UseDishesCoeff: true));





            //var Serv = new S2010.XrepSoapClient();
            //var DepList = Serv.GetPointList3();
            //Dictionary<int, int> ChecksCount = CheckCountSingletone.Instance.RestNonZeroOnlyCheckCount.GetCheckCount(day);
            //var resOut = new List<ReportDayResult>();
            //foreach (S2010.DepartmentInfo Dii in DepList)
            //{
            //    if (ChecksCount.TryGetValue(Dii.Number, out int chCount))
            //    {
            //        var DCount = DishesCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                    
            //        var res = new ReportDayResult()
            //        {
            //            BD = day,
            //            TypeId = TypeId,
            //            Dep = Dii.Number,
            //            DepName = Dii.Name,
            //            Count = chCount,
            //            Summ = (double)(DCount),
            //            Value = chCount != 0 ? (double)((DCount) / chCount) : 0
            //        };
            //        resOut.Add(res);
            //    }
            //}
            //return resOut;
        }

    }
}
