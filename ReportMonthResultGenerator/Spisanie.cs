using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;


namespace ReportMonthResultGenerator
{
    static class Spisanie
    {

        public static void SpisListToExcel(List<SpisDish> Spis)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook Wb = app.Workbooks.Add(true);
            Microsoft.Office.Interop.Excel.Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Cells[1, 1] = "Баркод";
            Ws.Cells[1, 2] = "Название";

            
            int row = 2;
            int col = 3;
            List<string> Deps = Spis.Select(a => a.DepName).Distinct().ToList();
            foreach(string s in Deps)
            {
                    Ws.Cells[col, 1] = s;
                col++;
            }
            
                foreach (SpisDish  Dish in Spis.Distinct(new DishComparer ()))
                {
                    Ws.Cells[1, row] = Dish.BarCode;
                    Ws.Cells[2, row] = Dish.DishName;
                    col = 3;
                    foreach (string s in Deps)
                    {
                        double res = (from a in Spis where a.BarCode == Dish.BarCode && a.DepName == s select a.Count ).Sum();
                        Ws.Cells[col, row] = res;
                        col++;
                    }

                    row++;
                }
                
            


            app.Save(System.Reflection.Missing.Value);
            app = null;
        }



        public static void GetDesertsSpisByDep(DateTime Fdt, DateTime Edt, int Dep)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            Ws.Cells[1, 1] = "Десерт";
            Ws.Cells[1, 3] = "Поставлено";
            Ws.Cells[1, 4] = "Списано";
            
            Edt = Edt.AddDays(-1);

            
                //if (Dii.Number == 262) continue;
                ReportMonthResult DesSpis = new ReportMonthResult();

                List<int> Deserts = DesertsOnStop.GetDesertList(Dep);

                WebSrvSpisanie.Ges3ServicesUTF8ObjClient Cl = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();
                decimal? cGood = 0;
                string cMeas = "";
                decimal? qPr = 0;
                decimal? qSp = 0;


                decimal SummPrihod = 0;
                decimal SummSpis = 0;
                List<decimal> LocalCodes = new List<decimal>();
                int row = 2;
                foreach (int Bc in Deserts)
                {
                    Cl.PrihAndSpis(Bc.ToString(), Fdt, Edt, Dep, out cGood, out cMeas, out qPr, out qSp);
                    if (LocalCodes.Contains(cGood.Value))
                    {
                        continue;
                    }
                    if (cMeas != "шт")
                    {
                        continue;
                    }
                if ((qPr == 0) && (qSp == 0))
                {
                    continue;
                }

                
                LocalCodes.Add(cGood.Value);
                    Ws.Cells[row, 1] = Bc;
                    Ws.Cells[row, 2] = DesertsOnStop.GetDishName(Bc);
                    Ws.Cells[row, 3] = qPr.Value;
                    Ws.Cells[row, 4] = qSp.Value;

                    //SummPrihod += qPr.Value;
                    //SummSpis += qSp.Value;
                    row++;
                }
                
            

           
        }



        public static List<ReportMonthResult> GetDesertsSpis(DateTime Fdt, DateTime Edt)
        {
            Edt = Edt.AddDays(-1);
            List<ReportMonthResult> Tmp = new List<ReportMonthResult>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                //if (Dii.Number != 205) continue;
                ReportMonthResult DesSpis = new ReportMonthResult();
                DesSpis.Department = Dii.Number;
                DesSpis.DepName = Dii.Name;
                DesSpis.Month = Fdt;

                List<int> Deserts = DesertsOnStop.GetDesertList(Dii.Number);
                //WebSrvSpisanie.Ges3ServicesUTF8ObjClient Cl = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();
                Ges3.Ges3ServicesObjClient Cl = new Ges3.Ges3ServicesObjClient();
                decimal? cGood = 0;
                string cMeas = "";
                decimal? qPr = 0;
                decimal? qSp = 0;
                decimal SummPrihod = 0;
                decimal SummSpis = 0;
                //List<decimal> LocalCodes = new List<decimal>();
                var bcs = new Ges3.PrihAndSpisTable_T_barcRow[Deserts.Count+1];

                int row = 0;
                foreach (int Bc in Deserts)
                {
                    bcs[row++] = new Ges3.PrihAndSpisTable_T_barcRow() { bCod = Bc.ToString() };
                }

                    int tryCount = 0;
                    while (tryCount < 10)
                    {
                        try
                        {
                            Cl.PrihAndSpisTable(bcs, Fdt, Edt, Dii.Number,   out qPr, out qSp);
                            tryCount = 10;
                        }
                        catch (Exception e)
                        {
                            tryCount++;
                        }
                    }
                    /*
                    if (LocalCodes.Contains(cGood.Value))
                    {
                        continue;
                    }
                    if (cMeas != "шт")
                    {
                        continue;
                    }
                    */
                    //LocalCodes.Add(cGood.Value);
                    SummPrihod += qPr.Value;
                    SummSpis += qSp.Value;
               
                DesSpis.Value = (double)SummPrihod;
                DesSpis.Value2 = (double)SummSpis;
                DesSpis.Value3=0;
                if (SummPrihod != 0)
                {
                    DesSpis.Value3 = (double)SummSpis / (double)SummPrihod;
                }

                Tmp.Add(DesSpis);
            }

            return Tmp;
        }

        public static List<SpisDish> GetSousSpis(DateTime Fdt, DateTime Edt)
        {
            List<SpisDish> ret = new List<SpisDish>();
            Ges3.Ges3ServicesObjClient s1 = new Ges3.Ges3ServicesObjClient();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
           S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

           
           foreach (S2010.DepartmentInfo Dii in DepList)
           {
               if (!Dii.Enabled) continue;
               Ges3.SouseLost_tmp2Row [] res  = new Ges3.SouseLost_tmp2Row[1000];
               string s= s1.SouseLost(Dii.Number, Fdt, Edt, out res);
               foreach (Ges3.SouseLost_tmp2Row r in res)
               { 
                if (!(r ==null))
                   {
                       SpisDish Sd = new SpisDish()
                       {
                           BarCode = (int)r.cg,
                           Dep = Dii.Number,
                           DepName = Dii.Name,
                           DishName = Encoding.UTF8.GetString(
                                Encoding.GetEncoding(1251).GetBytes(r.cNAME)), 
                           Count = (double)r.quanty,
                           Summ = (double)r.summa
                       };
                       ret.Add(Sd);
                   }
                
               }

           }
           return ret;
        }
    

        
    }
    public class SpisDish
    {
        public int Dep { set; get; }
        public string DepName { set; get; }
        public int BarCode { set; get; }
        public string DishName { set; get; }
        public double Count { set; get; }
        public double Summ { set; get; }

    }
    class DishComparer : IEqualityComparer<SpisDish>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(SpisDish x, SpisDish y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.BarCode == y.BarCode;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(SpisDish product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            

            //Get hash code for the Code field.
            int hashProductCode = product.BarCode.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductCode;
        }

    }

}
