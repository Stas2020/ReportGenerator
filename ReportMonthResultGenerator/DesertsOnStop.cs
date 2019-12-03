using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
    static class DesertsOnStop
    {
        static ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
        static public List<int> GetDesertList(int DepNum)
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            
            List<int> Res = null;
            if (DepNum == 0)
            {
                Res = (from o in RepBase.AlohaMenuItemCatsLinks where (o.CatNum == 54) && (DepList.Where(a=>a.Enabled).Select(a=>a.Number).Contains(o.DepId.Value)) select o.BarCode.Value).Distinct().ToList();
            }
            else
            {
                Res = (from o in RepBase.AlohaMenuItemCatsLinks where (o.CatNum == 54) && (o.DepId == DepNum) select o.BarCode.Value).Distinct().ToList();
            }
            return Res;
        }


        static public string GetDishName(int DishNum)
        {
            string Res = "";
            try
            {

                Res = (from o in RepBase.DishLists where o.DishBarCode == DishNum select o.DishName).First();
            }
            catch
            { 
            
            }
            
            return Res;
        }
        static public List<DesertsOnStopResult> GetDesertsOnStop(DateTime StartDate, DateTime StopDate)
        {
            List<DesertsOnStopResult> Tmp = new List<DesertsOnStopResult>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            foreach (S2010.DepartmentInfo Dii in DepList)
            {
               if (!Dii.Enabled) continue;
               if (Dii.Number==262) continue;
               
               if (Dii.Number == 240) continue;
               if (Dii.Number == 222) continue;
               DesertsOnStopResult res = new DesertsOnStopResult();
               res.Dep = Dii.Number;
               res.DepName = Dii.Name;
               foreach (int BarCode in GetDesertList(Dii.Number))
               {
                   if ((BarCode == 830) || (BarCode == 892395)
                           || (BarCode == 811244)
                       || (BarCode == 811248)
                       || (BarCode == 811263)
                       || (BarCode == 811652)

                       )
                   {
                       continue;
                   }
                   for (DateTime dt = StartDate; dt < StopDate; dt = dt.AddDays(1))
                   {
                       int TimeOnStop = RepBase.DiffTime4(dt.AddHours(8), dt.AddHours(22), Dii.Number, BarCode).Value;

                       res.MinOnStop += TimeOnStop;
                   }

               }

               Tmp.Add(res);
           }
            return Tmp;
        }


    }
    public class DesertsOnStopResult
    {
        public DesertsOnStopResult()
        {}
        public int Dep;
        public string DepName;
        public double MinOnStop;
    }
}
