using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator.AutoCalc
{
    public class CheckCountSingletone
    {
        private CheckCountSingletone()
        {
            AllCheckCount = new DataDicCasheByDayCorrection(a => CubeData.GetAllChkCountByDay(a));
            RestNonZeroOnlyCheckCount = new DataDicCasheByDayCorrection(a => CubeData.GetChecksCountWithoutDeleveryByDay(a));
            //AllCheckCountWOTables = new DataDicCasheByDayCorrection(a => CubeData.GetChecksCountWithoutDeleveryByDay(a, true));
        }
        static CheckCountSingletone instance;
        public static CheckCountSingletone Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CheckCountSingletone();
                }
                return instance;
            }
        }
        public DataDicCasheByDayCorrection AllCheckCount;
        public DataDicCasheByDayCorrection RestNonZeroOnlyCheckCount;
        //public DataDicCasheByDayCorrection AllCheckCountWOTables;


        public class DataDicCasheByDayCorrection

        {
            public DataDicCasheByDayCorrection(Func<DateTime, Dictionary<int, int>> _calcFunc)
            {
                parent = new DataDicCasheByDay<int>(_calcFunc);

            }

            DataDicCasheByDay<int> parent;

            public Dictionary<int, int> GetCheckCount(DateTime dt)
            {
                var res = CorrectTwinDep.Correct(parent.GetCheckCount(dt));
                return res;
            }
            public Dictionary<int, int> GetCheckCountWO900Tables(DateTime dt)
            {
                var res = CorrectTwinDep.Correct(parent.GetCheckCount(dt));
                return res;
            }

            /*
            private void CorrectTwinDep(Dictionary<int, int> data)
            {
                data = 
                Dictionary<int, int> twinDeps = new Dictionary<int, int>() { { 111, 121 }, { 122, 123 }, { 190, 191 } };
                foreach (var td in twinDeps)
                {
                    if (data.TryGetValue(td.Key, out int val))
                    {
                        if (data.TryGetValue(td.Value, out int valTwin))
                        {
                            data[td.Key] = val + valTwin;
                        }
                    }
                }
            }
            */
        }

        private class DataDicCasheByDay<T>
            where T : IComparable
        {
            Func<DateTime, Dictionary<int, T>> calcFunc;
            public DataDicCasheByDay(Func<DateTime, Dictionary<int, T>> _calcFunc)
            {
                calcFunc = _calcFunc;
            }

            private Dictionary<DateTime, Dictionary<int, T>> AllCheckCount = new Dictionary<DateTime, Dictionary<int, T>>();
            object AllCheckCountLocker = new object();
            public Dictionary<int, T> GetCheckCount(DateTime dt)
            {
                Dictionary<int, T> checkCountData;// = new Dictionary<int, int>();
                lock (AllCheckCountLocker)
                {
                    if (!AllCheckCount.TryGetValue(dt, out checkCountData))
                    {
                        var cubeData = new CubeData();
                        checkCountData = calcFunc.Invoke(dt);
                        AllCheckCount.Add(dt, checkCountData);
                    }

                }
                return checkCountData;
            }


        }
        }
        public static class CorrectTwinDep
        {
            static Dictionary<int, List<int>> twinDeps = new Dictionary<int, List<int>>() { 
                { 114, new List<int>() { 121, 111 } }, 
                { 122, new List<int>() { 123 } },
                { 190, new List<int>() { 191 } },
                { 124, new List<int>() { 104 } },
                { 331, new List<int>() { 311 } },
                { 242, new List<int>() { 300 } },
                { 244, new List<int>() { 231 } },
                { 301, new List<int>() { 380 } },
                { 302, new List<int>() { 270 } },
                { 276, new List<int>() { 205 } },
                { 277, new List<int>() { 177 } },
                { 278, new List<int>() { 390 } },
                { 281, new List<int>() { 295 } }, //add 2022-10-11
                { 282, new List<int>() { 180 } }, //add 2022-10-11
            };

            public static int GetDep(int _dep)
            {
                if (twinDeps.Any(_td => _td.Value.Contains(_dep)))
                    return twinDeps.First(_td => _td.Value.Contains(_dep)).Key;
                else
                    return _dep;
            }

            public static List<DishCount> Correct(List<DishCount> data)
            {
                foreach (var td in twinDeps)
                {
                    foreach (DateTime dt in data.Select(a => a.dt).Distinct())
                    {
                        var ndata = data.Where(a => a.dt == dt);
                        foreach(var tdVal in td.Value)
                        if (ndata.Any(a => a.Dep == td.Key) && ndata.Any(a => a.Dep == tdVal))
                        {

                            data.FirstOrDefault(a => a.Dep == td.Key && a.dt == dt).Count += ndata.Where(a => a.Dep == tdVal).Sum(a => a.Count);
                            data.FirstOrDefault(a => a.Dep == td.Key && a.dt == dt).MoneyCount += ndata.Where(a => a.Dep == tdVal).Sum(a => a.MoneyCount);

                            
                        }
                    }
                }
                return data;
            }

        public static List<RashMaterials> Correct(List<RashMaterials> data)
        {
            //2022-10-12 убираем цикл по записям в таблице корректировки twinDeps, иначе записи в data переносятся в новое подразделение столько раз какова длина списка подразделений с двойными номерами
            //foreach (var td in twinDeps)
            {
                foreach (var depData in data)
                {
                    if (twinDeps.TryGetValue(depData.Dep, out List<int> twDep))
                    {
                        foreach (var tdVal in twDep)
                            if (data.Any(a => a.Dep == tdVal))
                        {
                            ////depData.Checks += data.FirstOrDefault(a => a.Dep == twDep).Checks;
                            //depData.Value += data.Where(a => a.Dep == twDep).Sum(a=>a.Value);

                            depData.Consumables += data.FirstOrDefault(a => a.Dep == tdVal).Consumables;
                            depData.Proceeds += data.FirstOrDefault(a => a.Dep == tdVal).Proceeds;
                        }
                    }                   
                }
            }
            return data;
        }
        public static Dictionary<int, int> Correct(Dictionary<int, int> data)
            {
                
                foreach (var td in twinDeps)
                {
                    if (data.TryGetValue(td.Key, out int val))
                {
                    foreach (var tdVal in td.Value)
                    {
                        data.TryGetValue(td.Key, out val);
                        if (data.TryGetValue(tdVal, out int valTwin))
                        {
                            data[td.Key] = val + valTwin;
                        }
                    }
                    }
                }
                return data;
            }
        public static Dictionary<int, Productivity.ProductivityCalculatedValues> Correct(Dictionary<int, Productivity.ProductivityCalculatedValues> data)
        {
            foreach (var td in twinDeps)
            {
                foreach (var tdVal in td.Value)
                    if (data.ContainsKey(td.Key) && data.ContainsKey(tdVal))
                {
                    //foreach(var keyPair in Productivity.ProductivityCalculatedValues.)
                    data[td.Key].Values[typeof(AutoCalc.ProductivBarista)] = ((data[td.Key].Values[typeof(AutoCalc.ProductivBarista)] ?? 0) + (data[tdVal].Values[typeof(AutoCalc.ProductivBarista)] ?? 0));
                    data[td.Key].Values[typeof(AutoCalc.ProductivSeller)] = ((data[td.Key].Values[typeof(AutoCalc.ProductivSeller)] ?? 0) + (data[tdVal].Values[typeof(AutoCalc.ProductivSeller)] ?? 0));
                    data[td.Key].Values[typeof(AutoCalc.ProductivCook)] = ((data[td.Key].Values[typeof(AutoCalc.ProductivCook)] ?? 0) + (data[tdVal].Values[typeof(AutoCalc.ProductivCook)] ?? 0));
                }
            }
            return data;
        }

        public static List<SpisPercent> Correct(List<SpisPercent> data)
        {
            //2022-10-12 убираем цикл по записям в таблице корректировки twinDeps, иначе записи в data переносятся в новое подразделение столько раз какова длина списка подразделений с двойными номерами
            //foreach (var td in twinDeps)
            {
                foreach (var depData in data)
                {
                    if (twinDeps.TryGetValue(depData.Dep, out List<int> twDep))
                    {
                        foreach (var tdVal in twDep)
                            if (data.Any(a => a.Dep == tdVal))
                            {
                                ////depData.Checks += data.FirstOrDefault(a => a.Dep == twDep).Checks;
                                //depData.Value += data.Where(a => a.Dep == twDep).Sum(a=>a.Value);

                                var twinData = data.FirstOrDefault(a => a.Dep == tdVal && a.BD == depData.BD);
                                if (twinData != null)
                                {
                                depData.Producted += twinData.Producted;
                                depData.WrittenOff += twinData.WrittenOff;
                                }
                            }
                    }
                }
            }
            return data;
        }
    }
    
}