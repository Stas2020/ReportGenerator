using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace ReportMonthResultGenerator
{
    class AlkReport
    {
        public static void GetAlcReport()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            DateTime FSDate = new DateTime( 2014,6,1);
            DateTime FEDate = new DateTime( 2014,9,1);

            DateTime ESDate = new DateTime(2015,6,1);
            DateTime EEDate = new DateTime( 2015,9,1);

            string RedVine =  CubeData.GetDishesBkofAllLockal(CubeData.GetDishofSalesCatStr(20));
            string WhiteVine = CubeData.GetDishesBkofAllLockal(CubeData.GetDishofSalesCatStr(3));
            string Alk = CubeData.GetDishesBkofAllLockal(CubeData.GetDishofSalesCatStr(19));
            int row = 2;
            List<int> NotDep = new List<int> { 230, 300, 240, 212, 213, 180,255 };
            for (DateTime dt = FSDate; dt < FEDate; dt = dt.AddMonths(1))
            {
                List<DishCount> RedVineL = CubeData.GetDishesCount(dt, RedVine, false);
                List<DishCount> WhiteVineL = CubeData.GetDishesCount(dt,WhiteVine, false);
                List<DishCount> AlcL = CubeData.GetDishesCount(dt, Alk, false);
                List<DishCount> AllD = CubeData.GetDishesCount(dt, Alk, true);

                  Ws.Cells[row, 1]=RedVineL.Where(b=> (!NotDep.Contains(b.Dep))).Sum(a=>a.MoneyCount);
                  Ws.Cells[row, 2] = WhiteVineL.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                  Ws.Cells[row, 3] = AlcL.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                  Ws.Cells[row, 4] = AllD.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                  row++;

            }
            row++;
            for (DateTime dt = ESDate; dt < EEDate; dt = dt.AddMonths(1))
            {
                List<DishCount> RedVineL = CubeData.GetDishesCount(dt, RedVine, false);
                List<DishCount> WhiteVineL = CubeData.GetDishesCount(dt, WhiteVine, false);
                List<DishCount> AlcL = CubeData.GetDishesCount(dt, Alk, false);
                List<DishCount> AllD = CubeData.GetDishesCount(dt, Alk, true);

                Ws.Cells[row, 1] = RedVineL.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                Ws.Cells[row, 2] = WhiteVineL.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                Ws.Cells[row, 3] = AlcL.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                Ws.Cells[row, 4] = AllD.Where(b => (!NotDep.Contains(b.Dep))).Sum(a => a.MoneyCount);
                row++;
            }
        }
    }
}
