using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class DishesCalcGuest : CalcBase
    {
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
                    //var newCount = (decimal)data.ChecksCount * data.GuestOnCheck;
                    //var newVal = (decimal)(newCount != 0 ? (decimal)(data.DishesCount / newCount) : (decimal)0);
                    var res = new ReportDayResult()
                    {
                        BD = day,
                        TypeId = TypeId,
                        Dep = Dii.Number,
                        DepName = Dii.Name,
                        Count = data.GuestCount,// 1, //newCount,
                        Summ = (double)data.DishesCount,// (double)newVal, //(double)dishesCount,
                        Value = (double)(data.GuestCount != 0 ? data.DishesCount/data.GuestCount : 0) //(double)newVal
                    };
                    resOut.Add(res);
                }
            }
            return resOut;



            //// /////////////// IT's OLD ///////////////////////

            //var DishesCalcReportId = 7;            

            //var db = new ReportBaseDataContext();

            //var dishesRes = db.ReportDayResult.Where(a => a.BD == day && a.TypeId == DishesCalcReportId);

            //var Serv = new S2010.XrepSoapClient();
            //var DepList = Serv.GetPointList3();
            //var resOut = new List<ReportDayResult>();
            //foreach (S2010.DepartmentInfo Dii in DepList)
            //{
            //    //if (Dii.Number != 290)
            //    //    continue;
            //    //if (Dii.Number != 177 && Dii.Number != 371 && Dii.Number != 395 && Dii.Number != 375)
            //    //    continue;
            //    var dishesResDep = dishesRes.FirstOrDefault(_res => _res.Dep == Dii.Number);
            //    if (dishesResDep == null)
            //        continue;
            //    var dishesCount = dishesResDep.Summ;
            //    var checksCount = dishesResDep.Count;

            //    List<int> excludeTablesList = new List<int>();
            //    DishesCalc.excludeTables.ForEach(_list => { for (int i = _list.Min; i <= _list.Max; i++) excludeTablesList.Add(i); });

            //    // ToDo - правильно ли расчитывать по бизнес-дате??? (по ней беру сред кол-во гостей на чек)
            //    var avgGuestsOnCheckQuery = db.GuestCounts.Where(_cnt => _cnt.DepNum == Dii.Number && _cnt.BusinessDate == day && _cnt.Guests < 30  // кол-во гостей более 100 - дикая ошибка, но в кубах бывает
            //        && (_cnt.TableNumber == null || !excludeTablesList.Contains(_cnt.TableNumber.Value)));
            //    var avgGuestsOnCheck = avgGuestsOnCheckQuery.Count() > 0 ? (double)avgGuestsOnCheckQuery.Average(_cn => (double)_cn.Guests) : 0;

            //    var newCount = checksCount * avgGuestsOnCheck;
            //    var newVal = newCount != 0 ? (double)(dishesCount / newCount) : 0;
            //    if (newVal >= DishesCalc.minCorrectValue)
            //    {
            //        var res = new ReportDayResult()
            //        {
            //            BD = day,
            //            TypeId = TypeId,
            //            Dep = Dii.Number,
            //            DepName = Dii.Name,
            //            Count = 1, //newCount,
            //            Summ = newVal, //(double)dishesCount,
            //            Value = newVal
            //        };
            //        resOut.Add(res);
            //    }
            //}
            //return resOut;
        }

    }
}
