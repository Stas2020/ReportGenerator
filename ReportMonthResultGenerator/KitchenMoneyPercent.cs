using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace ReportMonthResultGenerator
{
    public static class KitchenMoneyPercent
    {



        public static void StoykaDishCount(DateTime Month)
        {
            //Бар 
            List<int> KPos = new List<int>() { 5, 6,12 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;

            List<DishCount> Dk = CubeData.GetStoikaDishesCount(Month);
            List<DishCount> AllDk = CubeData.GetAllDishesCount(Month);
            int row = 2;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                try
                {
                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    Ws.Cells[row, 3] = AllDk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    //Ws.Cells[row, 3] = CountInDay / 31;
                    //Ws.Cells[row, 4] = AllEmpl.Sum(a => a.sal)*30;
                    //Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }
            

            app.Save(System.Reflection.Missing.Value);

            app = null;
        }



        public static void KithenDishCount(DateTime Month)
        {
            List<int> KPos = new List<int>() { 2, 8 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;

            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);
            List<DishCount> AllDk = CubeData.GetAllDishesCount(Month);
            int row = 2;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                try
                {
                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    Ws.Cells[row, 3] = AllDk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    //Ws.Cells[row, 3] = CountInDay / 31;
                    //Ws.Cells[row, 4] = AllEmpl.Sum(a => a.sal)*30;
                    //Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }


            app.Save(System.Reflection.Missing.Value);

            app = null;
        }

    }
}
