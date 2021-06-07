using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            
            if (args.Length > 0)
            {

                if (args[0] == "WP")
                {
                    if (args.Length > 1)
                    {
                        int DC = int.Parse(args[1]);
                        WaiterPower.GenWaiterPower(DateTime.Now.Date.AddDays(-1 * DC), DateTime.Now.Date);
                    }
                    
                }
                if (args[0] == "?")
                {
                    Console.WriteLine("EveryNight");
                }
                else if (args[0] == "KitchenSOS")
                {

                    int dDeep = -5;
                    try
                    {
                        if (args.Length > 1)
                        {
                            dDeep = -Convert.ToInt32(args[1]);

                        }
                    }
                    catch { }


                    DateTime EEndDate = DateTime.Now.Date;
                    DateTime EStartDate = EEndDate.AddDays(dDeep);
                    
                    
                    Utils.ToLog($"Запуск EveryNight EStartDate: {EStartDate}; EEndDate: {EEndDate}",true);

                    //QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"C:\SoS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
                    QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\nas\winshare\SOS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate); 
                    ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);
                }
                else if (args[0] == "Pah")
                {
                    

                   Utils.ToLog("Запуск Пахаря ");
                  DateTime EStartDate = new DateTime(2019, 10, 01);
                  
                   
                   MainClass.UpdateDesertSpisaniePercent(EStartDate); 
                   MainClass.UpdateOrderTimePercent(EStartDate);
                   
                   MainClass.UpdateOrderTimeWODeliveryPercent(EStartDate);

                   MainClass.UpdateRashMatPercent(EStartDate);
                  

                   ExcelRepGenerate.ExcelGen(EStartDate);
                }
                else if (args[0] == "WTTest")
                {
                    WorkTimeOnChk.GetWorkTimeOnChk(new DateTime(2016, 11, 1));
                }
                else if (args[0] == "Kitchen")
                {
                  
                    DateTime EEndDate = new DateTime(2019,12,01);
                    DateTime EStartDate = new DateTime(2019, 11, 01);
                    
                  //DateTime EStartDate = EEndDate.AddMonths(-1);

/*
                  QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\s2010\SoS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
                  ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);
                  */
                    StaffWtToExcel.GenRepKitchenTmp(EStartDate);
                    Console.ReadKey();
                    //StaffWtToExcel.GenRepKitchenTmpYear(new DateTime(2018, 01, 1));
                }
                else if (args[0] == "RepsForDN")
                {
                    //      StaffWtToExcel.GenRepKitchen(new DateTime(2015, 12, 1));
                    ReportsDN.CalcEffect();
                    //DateTime EEndDate = new DateTime(2016, 2, 5);
                    //DateTime EStartDate = EEndDate.AddDays(-14);
                    //  ReportsDN.GenReportsFOT();
                    DateTime EEndDate = new DateTime(2018, 1, 1);
                    DateTime EStartDate = new DateTime(2018, 2, 1);
                    //TimeOfPreparation.GetAvgTimeOfPrepByCat(EEndDate, EStartDate, 57); // Холодные
                   // TimeOfPreparation.GetAvgTimeOfPrepByCat(EEndDate, EStartDate, 58); // Горячие
                    //ChecksCountOnPlace.GenRep(EStartDate, EEndDate);
                    //ChecksCountOnPlace.GenRepMoneyByDay(EStartDate, EEndDate);
                    //EStartDate = new DateTime(2015,12,01);
                    //EEndDate = new DateTime(2016, 1, 01);
                    //ChecksCountOnPlace.GenRepSalesOnPeople(EStartDate, EEndDate); 
                    //ChecksCountOnPlace.FotBarista(EStartDate, EEndDate); 
                   // ChecksCountOnPlace.GetRepFOTPercent(EStartDate, EEndDate); 

                    
                }
                else if (args[0] == "Barista")
                {
                    
                        StaffWtToExcel.GenrepStotka2(new DateTime(2019, 05, 1));
                        //StaffWtToExcel.GenrepStotkaByPeople(new DateTime(2016, 4, 1));
                       //StaffWtToExcel.GenrepStotkaSpecOnly(new DateTime(2018, 05, 1));
                
                }
                else if (args[0] == "PahDay")
                {
                    DateTime dtS = DateTime.Now.Date.AddDays(-1);
                    int dDeep = 1;
                    try
                    {
                        if (args.Length > 1)
                        {
                            dDeep= -Convert.ToInt32(args[1]);

                        }
                    }
                    catch { }
                    DateTime dtE = dtS.AddDays(dDeep);
                    Console.WriteLine($"Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}");

                    AutoCalc.Calculation.Init();
                    for (DateTime dt = dtS; dt >= dtE; dt = dt.AddDays(-1))
                    {
                        AutoCalc.Calculation.DayCalc(dt);
                    }

                }

            }
            else
            {

                WaiterPower.GenWaiterPower(DateTime.Now.Date.AddDays(-1), DateTime.Now.Date);

                // DateTime EEndDate = DateTime.Now.Date;
                // DateTime EStartDate = EEndDate.AddDays(-35);


                //Utils.ToLog($"Запуск EveryNight EStartDate: {EStartDate}; EEndDate: {EEndDate}", true);

                //QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"C:\SoS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
                //QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\nas\winshare\SOS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
             //   ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);


                /*
                Tasks.MenuAllItemsUpdaterTask t = new Tasks.MenuAllItemsUpdaterTask();
                t.Run();
                */








                //  ReportsDN.GetEffectByDayRange();
                // DbfData.DBFExtractor.GetAvgTimeOfTableNY();
                // DateTime dt = new DateTime(2018, 01, 05, 00, 02, 00);
                //  DateTime dt2 = dt.AddSeconds(-1515110520);
                //  Console.WriteLine(dt2.ToString());
                //  Console.Read();
                //HRFOOL.GetLastPosRepUpr();
                //HRFOOL.GetRepLitv();
                // HRFOOL.GetLastPosRepByPos(8);
                //HRFOOL.GetRepChik();
                //HRFOOL.GetRep();
                //HRFOOL.GetRepDissmiss();
                //DbfData.DBFExtractor.GetAvgTimeOfTableNY();
                //  WaiterPower.GetPepleCountOfHourToExcel();
                //  KissTheCook.TestReport(new DateTime(2018, 9, 1), new DateTime(2018, 10, 1));
                // KissTheCook.TestReportByDay(new DateTime(2018, 3, 14));
                //KissTheCook.TestReportByDayAllDishes(new DateTime(2018, 3, 14));
                //KissTheCook.GetAvgTimeDataRussia(new DateTime(2017, 8, 1), new DateTime(2017, 9, 1));
                //QSR.QSRTiming.GetAvgQSRPercentAllDir(@"E:\t\qsr\295\");
                //QSR.QSRTiming.InsertOrderTimeRecordsAllDir(@"E:\t\qsr\130\",130);


                //!!!!!!!!!!!!!!!!!!
                //   QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\s2010\SoS\");

                //ForNY.WorkTimeRepByWeeks();

                //Аэропорты рассчет скорости обслуживания на кассе
                //     AirReports.GetSpeedOfServTest(new DateTime(2018, 12, 1));

                //Аэропорты кол-во раб часов на чек
                //     AirReports.GetWTOnCheckTest(new DateTime(2018, 12, 1));


                //  QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"D:\SoS\");


                //QSR.QSRTiming.OtTest();
                //QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\S2004\Exchange\piskov\QSRData");

                // сRashMat.GetRashMatList(new DateTime(2017, 12, 1), new DateTime(2018, 1, 1));
                //DbfData.DBFExtractor.ExtractDBFs(new DateTime(2017, 7, 1));
                //   DbfData.DBFExtractor.ReportGen();
                //   ReportsDN.GenReportsPartSales();
                //  StaffWtToExcel.AvgStavka(new DateTime(2017, 3, 1));
                //  StaffWtToExcel.GenRepAll(new DateTime(2017, 3, 1));
                //   NYQSR.CreateReport();
                //  сRashMat.GetRashMatList(new DateTime(2017, 4, 1), new DateTime(2017, 5, 1));
                //  Console.WriteLine(CubeData.GetKitchenDishesCount2(new DateTime(2016, 12, 1)).Where(a=>a.Dep==380).Sum(a=>a.Count));
                // Console.Read();
                //сRashMat.GetByYear(2014);
                //   DateTime dt = new DateTime(2016, 7, 01);
                // DesertsOnStop.GetDesertsOnStop(dt, dt.AddMonths(1));
                //        VoidResult VR = new VoidResult ();
                //      VR.GetRes2();
                //  TimeOfPreparation.ReportGenerate(new DateTime(2016, 3, 12), new DateTime(2016, 3, 16));

                //    TimeOfPreparation.GetAvgTimeOfPrepByDeps(new DateTime(2016, 4, 01), new DateTime(2016, 4, 14));

                //     DesertsOnChk.GetDesertsSaleReport(new DateTime(2015, 11, 23), new DateTime(2015, 11, 29), 371);
                //DateTime EStartDate = new DateTime(2015, 11, 1);
                //   MainClass.UpdateRashMatPercent(EStartDate);
                /*
                DateTime EStartDate = DateTime.Now.Date.AddDays(-7);
                DateTime EEndDate = DateTime.Now.Date;
                ReportTableUpdater.OrderTimeBySuSheff(TimeOfPreparation.GetTimeOfPrepByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);
              //  AlkReport.GetAlcReport();
                /*
                DateTime EStartDate = new DateTime(2015, 8, 12);
                DateTime EEndDate = new DateTime(2015, 8, 12);

                WaiterPower.GenWaiterPower(EStartDate, EEndDate);
                ReportTableUpdater.OrderTimeBySuSheff(TimeOfPreparation.GetTimeOfPrepByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);
                */
                //    DateTime EStartDate = new DateTime(2015, 9, 1);
                //MainClass.UpdateDesertSpisaniePercent(EStartDate);
                //ExcelRepGenerate.ExcelGen(EStartDate);
                //  KitchenMoneyPercent.StoykaDishCount(new DateTime(2015, 7, 1));
                //   KitchenMoneyPercent.KithenDishCount(new DateTime(2015, 8, 1));
                //   StaffWtToExcel.GenRepKitchen(new DateTime(2015, 12, 1));
                //   StaffWtToExcel.GenRepkassirwt(new DateTime(2015, 10, 1));

                //  StaffWtToExcel.GenRepNapitki(new DateTime(2015, 10, 1));
                //StaffWtToExcel.GenRepStoika2(new DateTime(2015, 10, 1));
                //StaffWtToExcel.GenRepPovarcex(new DateTime(2015, 8, 1));
                // StaffWtToExcel.GenrepMoneyPerHour(new DateTime(2016, 10, 1));
                // StaffWtToExcel.GenrepDeserts(new DateTime(2016, 01, 1));
                //     Spisanie.GetDesertsSpisByDep(new DateTime(2018, 10, 01), new DateTime(2018, 11, 01), 260);

                //     StaffWtToExcel.Genrep(new DateTime(2015, 9, 1));
                //    StaffWtToExcel.GenrepStotka(new DateTime(2015, 7, 1));
                //   StaffWtToExcel.GenrepStotkaWithoutKofe(new DateTime(2015, 7, 1));

                //WaiterPower.GenWaiterPower();
                /*
                MainClass.UpdateOrderTimePercent(new DateTime(2015, 8, 1));
                MainClass.UpdateDesertSpisaniePercent (new DateTime(2015, 8, 1));
                MainClass.UpdateRashMatPercent(new DateTime(2015, 8, 1));

                 MainClass.UpdateDesertonChk(new DateTime(2015, 8, 1));
                MainClass.UpdateDesertsOnStopTime(new DateTime(2015, 8, 1));
                */

                // MainClass.GenSousReport(new DateTime(2016, 2, 1));
                //   CoffeeToGo.GetCoffeeTogo(new DateTime(2016, 2, 1));
                //MainClass.UpdateDissmissPercent(new DateTime(2015,1,1));
                //ExcelRepGenerate.ExcelGen(new DateTime(2015, 7, 1));

                //BaristaLong.ExcelBaristaGen();
                //TimeOfPreparation.GetTimeOfPrepByPointAndCat(new DateTime(2015, 2, 1), new DateTime(2015, 3, 1), 395, 10);


                /*
                                DateTime StartDate = new DateTime(2015, 7, 13);
                                DateTime EndDate = new DateTime(2015, 7, 23);
                                ReportTableUpdater.OrderTimeBySuSheff(TimeOfPreparation.GetTimeOfPrepByDayAndSuShef(StartDate, EndDate), StartDate, EndDate);
                 * */
            }

            //Fot.GetFot(new DateTime(2015, 3, 1), 104);

            //StaffWtToExcel.KithenDishCount(new DateTime(2015, 3, 1));
            // StaffWtToExcel.Genrep2(new DateTime(2015, 3, 1));
            //StaffWtToExcel.GenDecoratorsRep2(new DateTime(2015, 3, 1));
            //    StaffWtToExcel.GenrepForStoika(new DateTime(2013, 11, 1));
        }
    }
}
