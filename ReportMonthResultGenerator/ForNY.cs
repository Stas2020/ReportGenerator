using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportMonthResultGenerator
{
    public static  class ForNY
    {
        public static void WorkTimeRep()
        {
            var stPos = new List<int> { 5, 6, 12 };//Продавец, бариста, мен. стойки
            var kPos = new List<int> { 2, 4, 8 };//Повар, старший повар, су-шеф
            var holePos = new List<int> { 27, 15, 3, 149 };//Официант, хостес, пом. официанта, пом. зала,менеджер
            var cleaningPos = new List<int> { 1, 14, 148};//Посудомойщики, стюарды, уборщик ресторана

            var posDic = new Dictionary<int, List<int>>() { { 3, stPos }, { 4, kPos },{ 5, holePos },{ 6, cleaningPos } };

            var depDic = new Dictionary<int, string>() { { 104, "Никитская"}, { 370, "Кутузовский" }, { 295, "Комсомольский" }, { 380, "ГУМ" }, { 371, "Лубянка" }, { 390, "Лесная" } };
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            Ws.Cells[1, 2] = "Кол-во гостей";
            Ws.Cells[1, 3] = "Стойка";
            Ws.Cells[1, 4] = "Кухня";
            Ws.Cells[1, 5] = "Зал";
            Ws.Cells[1, 6] = "Мойка + уборщицы";
            Cube2005DataContext db = new Cube2005DataContext();
            db.CommandTimeout = 0;
          //  db.Connection.ConnectionTimeout = 0;
            int rowNum = 2;
            for (int m = 1; m < 13; m++)
            {

                Ws.Cells[rowNum, 1] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m);

                rowNum++;
                DateTime Month = new DateTime(2018, m, 1);
                
                foreach (int dNum in depDic.Keys)
                {
                    int GCount = db.XmlChecks.Where(a => a.Dep == dNum && a.SystemDate >= Month && a.SystemDate < Month.AddMonths(1) && a.Guests<20).Sum(a => a.Guests.GetValueOrDefault(1));
                    string dName = depDic[dNum];
                    Ws.Cells[rowNum, 1] = dName;
                    Ws.Cells[rowNum, 2] = GCount;
                    //List<CEmplWt> Wts = StaffBase.GetWtsByDep(dNum, Month, Month.AddMonths(1));
                    foreach (int col in posDic.Keys)
                    {
                        var poss = posDic[col];
                        List<CEmpl> PossEmpls = StaffBase.GetEmplsOfPos(Month, poss);
                        foreach (int p in poss)
                        {
                            PossEmpls.AddRange(StaffBase.getPeopleOfPosOld(p.ToString(), Month));
                        }

                        List<CEmplWt> Wts = StaffBase.GetWts(PossEmpls.Distinct().ToList(), Month, Month.AddMonths(1)); 

                        double hrs = Wts.Where(a => poss.Contains(a.Emp.Pos) && a.Dep==dNum).Sum(a => (StaffWtToExcel.GetMaxDate(a.StopDt, Month) - StaffWtToExcel.GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                        if (col == 5) { hrs += 30 * 24; }
                        Ws.Cells[rowNum, col] = hrs;

                    }
                    rowNum++;
                    
                }
                
                rowNum++;


            }

        }

        public static void WorkTimeRepByWeeks()
        {
            var stPos = new List<int> { 5, 6, 12 };//Продавец, бариста, мен. стойки
            var kPos = new List<int> { 2, 4, 8 };//Повар, старший повар, су-шеф
            var holePos = new List<int> { 27, 15, 3, 149 };//Официант, хостес, пом. официанта, пом. зала,менеджер
            var cleaningPos = new List<int> { 1, 14, 148 };//Посудомойщики, стюарды, уборщик ресторана

            var posDic = new Dictionary<int, List<int>>() { { 3, stPos }, { 4, kPos }, { 5, holePos }, { 6, cleaningPos } };

            var depDic = new Dictionary<int, string>() { { 104, "Никитская" }, { 370, "Кутузовский" }, { 295, "Комсомольский" }, { 380, "ГУМ" }, { 371, "Лубянка" }, { 390, "Лесная" } };
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            Ws.Cells[1, 2] = "Кол-во гостей";
            Ws.Cells[1, 3] = "Стойка";
            Ws.Cells[1, 4] = "Кухня";
            Ws.Cells[1, 5] = "Зал";
            Ws.Cells[1, 6] = "Мойка + уборщицы";
            Cube2005DataContext db = new Cube2005DataContext();
            db.CommandTimeout = 0;
            //  db.Connection.ConnectionTimeout = 0;
            int rowNum = 2;
            for (int m = 1; m < 2; m++)
            {
                for (int w = 0; w < 4; w++)
                {
                    Ws.Cells[rowNum, 1] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m) + "неделя: " +(w+1).ToString(); 
                    rowNum++;
                    DateTime Month = new DateTime(2018, m, w*7+1);

                    foreach (int dNum in depDic.Keys)
                    {
                        int GCount = db.XmlChecks.Where(a => a.Dep == dNum && a.SystemDate >= Month && a.SystemDate < Month.AddDays(7) && a.Guests < 20).Sum(a => a.Guests.GetValueOrDefault(1));
                        string dName = depDic[dNum];
                        Ws.Cells[rowNum, 1] = dName;
                        Ws.Cells[rowNum, 2] = GCount;
                        //List<CEmplWt> Wts = StaffBase.GetWtsByDep(dNum, Month, Month.AddMonths(1));
                        foreach (int col in posDic.Keys)
                        {
                            var poss = posDic[col];
                            List<CEmpl> PossEmpls = StaffBase.GetEmplsOfPos(Month, poss);
                            foreach (int p in poss)
                            {
                                PossEmpls.AddRange(StaffBase.getPeopleOfPosOld(p.ToString(), Month));
                            }

                            List<CEmplWt> Wts = StaffBase.GetWts(PossEmpls.Distinct().ToList(), Month, Month.AddDays(7));

                            double hrs = Wts.Where(a => poss.Contains(a.Emp.Pos) && a.Dep == dNum).Sum(a => (StaffWtToExcel.GetMaxDate(a.StopDt, Month) - StaffWtToExcel.GetMinDate(a.StartDt, Month.AddDays(7))).TotalHours);
                            if (col == 5) { hrs += 7 * 24; }
                            Ws.Cells[rowNum, col] = hrs;

                        }
                        rowNum++;

                    }

                    rowNum++;


                }

            }
        }

    }
}
