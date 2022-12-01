using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    class RashMatGroup1Calc : CalcBase
    {
        public RashMatGroup1Calc()
        { }

        public override List<ReportDayResult> Calc(DateTime day)
        {
            var res = CorrectTwinDep.Correct(сRashMatGroups.GetRashMatByDay(day, day.AddDays(1), 1));
            List<ReportDayResult> resOut = res.Select(a => new ReportDayResult()
            {
                BD = day,
                Count = a.Proceeds, //a.Checks,
                Dep = a.Dep,
                DepName = a.DepName,
                Summ = a.Consumables, //a.Value,
                Value = a.Value, // a.ValueOnCheck,
                TypeId = TypeId
            }).ToList();
            //res.ForEach(_r => {
            //    Utils.ToDebugLog($"RES_IN    {_r.Dep}   Value:{_r.Value}   ValueOnCheck:{_r.ValueOnCheck}   Checks:{_r.Checks}");
            //});
            //resOut.ForEach(_r => {
            //    Utils.ToDebugLog($"RES_OUT   {_r.Dep}   Summ:{_r.Summ}     Value:{_r.Value}   Count:{_r.Count}");
            //});
            return resOut;
        }
    }
}