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
            static Dictionary<int, int> twinDeps = new Dictionary<int, int>() { { 111, 121 }, { 122, 123 }, { 190, 191 } };
            public static List<DishCount> Correct(List<DishCount> data)
            {
                foreach (var td in twinDeps)
                {
                    foreach (DateTime dt in data.Select(a => a.dt).Distinct())
                    {
                        var ndata = data.Where(a => a.dt == dt);
                        if (ndata.Any(a => a.Dep == td.Key) && ndata.Any(a => a.Dep == td.Value))
                        {

                            data.FirstOrDefault(a => a.Dep == td.Key && a.dt == dt).Count += ndata.Where(a => a.Dep == td.Value).Sum(a => a.Count);
                            data.FirstOrDefault(a => a.Dep == td.Key && a.dt == dt).MoneyCount += ndata.Where(a => a.Dep == td.Value).Sum(a => a.MoneyCount);

                            
                        }
                    }
                }
                return data;
            }

        public static List<RashMaterials> Correct(List<RashMaterials> data)
        {
            foreach (var td in twinDeps)
            {
                foreach (var depData in data)
                {
                    if (twinDeps.TryGetValue(depData.Dep,out int twDep))
                    {
                        if (data.Any(a => a.Dep == twDep))
                        {
                            //depData.Checks += data.FirstOrDefault(a => a.Dep == twDep).Checks;
                            depData.Value += data.Where(a => a.Dep == twDep).Sum(a=>a.Value);
                            

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
                        if (data.TryGetValue(td.Value, out int valTwin))
                        {
                            data[td.Key] = val + valTwin;
                        }
                    }
                }
                return data;
            }
        }
    
}