using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;


namespace ReportMonthResultGenerator
{
    public static class ExcelRepGenerate
    {
        public static void ExcelGen(DateTime Month)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            //Ws.Cells[2, 1] = "Критерии";
            
            Ws.Cells[1, 2] = "Выполнение плана";
            Ws.Cells[1, 3] = "ФОТ";
            Ws.Cells[1, 4] = "Инвентаризация";
            Ws.Cells[1, 5] = "Доля списания десертов";
            Ws.Cells[1, 6] = "Расходные материалы (руб/чек)";
            Ws.Cells[1, 7] = "Время приготовления";

            Ws.Cells[1, 8] = "Продажа  напитков  на чек ";
            Ws.Cells[1, 9] = "Продажа блюд и десертов на чек";
            Ws.Cells[1, 10] = "Производительность труда";
            Ws.Cells[1, 11] = "Скорость движения очереди";
            Ws.Cells[1, 12] = "СанПин";

           


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            
            List<ReportMonthResult> RashMaterials = ReportTableUpdater.GetRashMaterials(Month);
            List<ReportMonthResult> OrderTime = ReportTableUpdater.GetOrderTime(Month);
            List<ReportMonthResult> DesertSpisanie = ReportTableUpdater.GetDesertSpisanie(Month);
            
            Dictionary<int, int> ChecksCount = CubeData.GetChecksCountWithoutDelevery(Month);
           
            //Кол-во блюд
            var DishesList  = CubeData.GetDishOfCat(41); //Блюда кухни
            DishesList.AddRange(CubeData.GetDishOfCat(54));// десерты
            var DishesCount = CubeData.GetDishesCountNoDelevery(Month, string.Join(",", DishesList.ToArray()));

            //Кол-во напитков в бокалах
            var DrinksListCup = CubeData.GetDishOfCat(92); //напитки в бокалах
            var DrinksCupCount = CubeData.GetDishesCountNoDelevery(Month, string.Join(",", DrinksListCup.ToArray()));

            //Кол-во напитков в бутылках
            var DrinksListBottle = CubeData.GetDishOfCat(91); //напитки в бутылках
            var DrinksBottleCount = CubeData.GetDishesCountNoDelevery(Month, string.Join(",", DrinksListBottle.ToArray()));

            
            var MoneyCount = CubeData.GetMoneyCount(Month);
            //Раб часы
            var wts = StaffWtToExcel.GetWtsNoWaiter(Month);

            var SPS = AirReports.GetSpeedOfServAvg(Month);
            //var SPS = new Dictionary<int, decimal> ();



            //   List<ReportMonthResult> DesertOnStop = ReportTableUpdater.GetDesertOnStop(Month);
            //   List<ReportMonthResult> DesertOnchk = ReportTableUpdater.GetDesertOnChk(Month);

            //  List<ReportMonthResult> WHOnChk = WorkTimeOnChk.GetWorkTimeOnChk(Month);

            /*
            int DesOnStopSum = (int)DesertOnStop.Sum(a => a.Value);
            double SrDesOnStopSum = 0;
            if (DesertOnStop.Where(a => a.Value > 0).Count() != 0)
            {
                SrDesOnStopSum = DesOnStopSum / DesertOnStop.Where(a => a.Value > 0).Count();
            }
            */

            /*
            List<ReportMonthResult> mCoffeeOnChk = new List<ReportMonthResult>();
            mCoffeeOnChk = CoffeeOnChk.GetCoffeeOnChk(Month);
            */
            var sanPin = AlWebApi.GetDepartStat();

            int col =2;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a =>a.Name))
            {
              if (!Dii.Enabled) continue;
                Ws.Cells[ col,1] = Dii.Name;


                try
                {
                    ReportMonthResult DesSpis = DesertSpisanie.Where(a => a.Department.Value == Dii.Number).First();
                    Ws.Cells[col, 5] = DesSpis.Value3;

                }
                catch
                {

                }



                try
                {
                    //double k=0;
                    //    if (SrCheck.TryGetValue(Dii.Number,out k))
                    //    {
                    //        Ws.Cells[col, 6] = (decimal)RashMaterials.Where(a => a.Department.Value == Dii.Number).First().Value3 * 1000 / (decimal)k;
                    //    }
                    //else
                    //    {
                    //        Ws.Cells[col, 6] = 0;
                    //    }
                    Ws.Cells[col, 6] = (decimal)RashMaterials.Where(a => a.Department.Value == Dii.Number).First().Value3.Value;
                    
                    
                }
                catch
                { }

                //Время приготовления
                try
                {
                    ReportMonthResult ORT = OrderTime.Where(a => a.Department.Value == Dii.Number).First();
                    double ORTD = 0;
                    if (ORT.Value2.Value != 0)
                    {
                        ORTD = (ORT.Value3.Value / ORT.Value2.Value);
                    }
                    if (ORTD != 0)
                    {
                        Ws.Cells[col, 7] = ORTD;
                    }
                }
                catch
                { }


                //Продажа  напитков  на чек
                try
                {
                    if (ChecksCount.TryGetValue(Dii.Number, out int chCount))
                    {
                        if (chCount > 0)
                        {
                            if (Dii.Number == 111)
                            {

                                var drcupCount = DrinksCupCount.Where(a => a.Dep == Dii.Number || a.Dep == 121).Sum(a => a.Count);
                                var drbottleCount = DrinksBottleCount.Where(a => a.Dep == Dii.Number || a.Dep == 121).Sum(a => a.Count);
                                ChecksCount.TryGetValue(121, out int chCount2);
                                Ws.Cells[col, 8] = (drcupCount + drbottleCount * 4) / (chCount+ chCount2);
                            }
                            else if (Dii.Number == 190)
                            {
                                var drcupCount = DrinksCupCount.Where(a => a.Dep == Dii.Number || a.Dep == 191).Sum(a => a.Count);
                                var drbottleCount = DrinksBottleCount.Where(a => a.Dep == Dii.Number || a.Dep == 191).Sum(a => a.Count);
                                ChecksCount.TryGetValue(191, out int chCount2);
                                Ws.Cells[col, 8] = (drcupCount + drbottleCount * 4) / (chCount + chCount2);
                            }
                            else
                            {
                                var drcupCount = DrinksCupCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                                var drbottleCount = DrinksBottleCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                                Ws.Cells[col, 8] = (drcupCount + drbottleCount * 4) / chCount;
                            }
                        }
                    }
                }
                catch
                { }

                //Продажа блюд и десертов на чек
                try
                {
                    if (ChecksCount.TryGetValue(Dii.Number, out int chCount))
                    {
                        if (chCount > 0)
                        {
                            var dishCount = DishesCount.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                            Ws.Cells[col, 9] = (dishCount) / chCount;
                        }
                    }
                }
                catch
                { }

                //Производительность труда
                try
                {

                    if (wts.TryGetValue(Dii.Number, out decimal hoursCount))
                    {
                        if (hoursCount > 0)
                        {
                            decimal moneyCount = 0;
                            if (MoneyCount.TryGetValue(Dii.Number, out moneyCount))
                            {

                                Ws.Cells[col, 10] = (moneyCount) / hoursCount;
                            }
                        }
                    }

                }
                catch
                { }

                
                //Скорость движения очереди
                try
                {
                    decimal spsdep = 0;
                    if (SPS.TryGetValue(Dii.Number, out spsdep))
                    {
                            Ws.Cells[col, 11] = spsdep;
                    }
                }
                catch
                { }

                //СанПин
                try
                {
                        var sanPinDep = sanPin.Where(a => a.DepId == Dii.Number).SelectMany(a => a.CheckListStats).SelectMany(a => a.Stats)
                            .Where(a => a.StatDate >= Month && a.StatDate < Month.AddMonths(1) && a.StatCompleted && a.StatRatio>70)
                        
                            .Average(a => a.StatRatio);
                    Ws.Cells[col, 12] = sanPinDep;


                }
                catch
                { }


                /*
                try
                {
                    ReportMonthResult DesOnSt = DesertOnStop.Where(a => a.Department.Value == Dii.Number).First();
                    if (DesOnSt.Value > 0)
                    {
                        Ws.Cells[col,10] = (DesOnSt.Value - SrDesOnStopSum) / SrDesOnStopSum;
                    }
                }
                catch
                { 
                
                }

                try
                {
                    if (mCoffeeOnChk.FirstOrDefault(a => a.Department == Dii.Number).Value != 0)
                    {
                        Ws.Cells[col, 11] = (double)(mCoffeeOnChk.FirstOrDefault(a => a.Department == Dii.Number).Value2) / (double)(mCoffeeOnChk.FirstOrDefault(a => a.Department == Dii.Number).Value);
                    
                    }
                
                }
                catch
                { }

                try
                {
                    ReportMonthResult DesOnChk = DesertOnchk.Where(a => a.Department.Value == Dii.Number).First();
                    Ws.Cells[col, 12] = DesOnChk.Value3;
                }
                catch
                {

                }
                try
                {
                    ReportMonthResult WHOnChkome = WHOnChk.Where(a => a.Department.Value == Dii.Number).First();
                    Ws.Cells[col, 13] = WHOnChkome.Value3;
                }
                catch
                {

                }
                try
                {
                    if (Dii.Place.ToLower().Trim() == "город")
                    {
                        double k = 0;
                        KitchenRep.TryGetValue(Dii.Number, out k);
                        Ws.Cells[col, 14] = k;
                    }
                    else
                    {
                        double k = 0;
                        SrCheck.TryGetValue(Dii.Number, out k);
                        double Lastk = 0;
                        SrCheckLast.TryGetValue(Dii.Number, out Lastk);
                        Double sr = (k - Lastk) / Lastk;
                        Ws.Cells[col, 9] = sr;
                    }
                    if (Dii.Number == 264)
                    {
                        double k = 0;
                        SrCheck.TryGetValue(Dii.Number, out k);
                        double Lastk = 0;
                        SrCheckLast.TryGetValue(Dii.Number, out Lastk);
                        Double sr = (k - Lastk) / Lastk;
                        Ws.Cells[col, 9] = sr;
                    }


                }
                catch
                { }

                try
                {
                        double k = 0;
                        StoykaRep.TryGetValue(Dii.Number, out k);
                        Ws.Cells[col, 15] = k;
                }
                catch
                { }

    */
                col++;
            }

            

            Ws.get_Range("A1:Z1").WrapText = true;
            Ws.get_Range("A1:Z1").HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                Ws.get_Range("A1:Z1").VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            //Ws.get_Range("B2:F30").NumberFormat = "0.00 р";
            Ws.get_Range("E2:E40").NumberFormat = "0.00 %"; 
            Ws.get_Range("F2:F40").NumberFormat = "0.00 р";
            Ws.get_Range("G2:G40").NumberFormat = "0.00 %";
            Ws.get_Range("H2:H40").NumberFormat = "0.00";
            Ws.get_Range("I2:I40").NumberFormat = "0.00";
            Ws.get_Range("J2:J40").NumberFormat = "0.00";
            Ws.get_Range("K2:K40").NumberFormat = "0.00";
            Ws.get_Range("L2:L40").NumberFormat = "0.00";
            Ws.get_Range("M2:M40").NumberFormat = "0.00";
            Ws.get_Range("N2:N40").NumberFormat = "0.00";
            Ws.get_Range("O2:O40").NumberFormat = "0.00";
            Ws.get_Range("A1:Z1").EntireColumn.AutoFit();

            for (int columnIndex = 2; columnIndex < 50; columnIndex++)
            {
               ((Range)Ws.Columns[columnIndex, Type.Missing]).EntireColumn.ColumnWidth = 12;
                //Ws.Columns[1,c]
            }

            app.Save(System.Reflection.Missing.Value);
            
            app = null;
        }

    }
}
