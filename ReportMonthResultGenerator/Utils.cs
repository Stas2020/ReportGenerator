using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ReportMonthResultGenerator
{
    public class Utils
    {
        public static void ToLog(string Mess, bool toConsole = false)
        {
            if (toConsole)
            {
                Console.WriteLine(Mess);
            }

            using (StreamWriter sw = new StreamWriter("log.txt",true))
            {
                sw.WriteLine(DateTime.Now.ToString(@"dd/MM/yyyy HH:mm:ss") + "  " + Mess);
            }
        }


    }
    public class StaffDismissal
    {
        public StaffDismissal()
        { }
        public int Dep { set; get; }
        public string DepName { set; get; }
        public int DismissCount { set; get; }
        public int StaffCount { set; get; }
        public double Percent
        {
            get
            {
                if (StaffCount == 0)
                {
                    return 0;
                }
                else
                {
                    return 100 * (double)DismissCount / (double)StaffCount;
                }
            }
        }
    }

    public class PrepTime
    {
        public PrepTime()
        { }
        public int Dep { set; get; }
        public string DepName { set; get; }
        public int AllCount { set; get; }
        public int WrongCount { set; get; }
        public int WrongSecond { set; get; }
        public int NormaSumm { set; get; }
        public int FactSumm { set; get; }

        public double Percent
        {
            get
            { 
                if (NormaSumm==0) return 0;
                return (FactSumm * 100 / NormaSumm);
            }
        }
    }
    
    public class RashMaterials
    {
        public RashMaterials()
        { }

        public int Dep { set;get; }
        public string DepName { set; get; }
        public double Value { set; get; }
        public double Checks { set; get; }
        public double ValueOnCheck
        {
            get
            {
                if (Checks != 0)
                {
                    return Value / Checks;
                }
                else
                {
                    return 0;
                }
                
            }
        }
    }
}
