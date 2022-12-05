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


                    Utils.ToLog($"НАЧАЛО РАБОТЫ - EveryNight (KitchenSOS) EStartDate: {EStartDate}; EEndDate: {EEndDate}", true);
                    //Utils.ToLog($"Запуск EveryNight EStartDate: {EStartDate}; EEndDate: {EEndDate}", true);

                    //QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"C:\SoS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
                    QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\nas\winshare\SOS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
                    ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);
                    Utils.ToLog($"УСПЕШНО ЗАВЕРШЕНО - EveryNight (KitchenSOS) {EStartDate.ToShortDateString()} - {EEndDate.ToShortDateString()}", true);
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

                    //DateTime EEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
                    //DateTime EStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 01);

                    //DateTime EEndDate = new DateTime(2021, 12, 31);
                    //DateTime EStartDate = new DateTime(2021, 01, 01);

                    DateTime EEndDate = new DateTime(2022, 12, 01);
                    DateTime EStartDate = new DateTime(2022, 11, 01);

                    //DateTime EStartDate = EEndDate.AddMonths(-1);

                    /*
                                      QSR.QSRTiming.InsertOrderTimeRecordsAllDeps(@"\\s2010\SoS\", (int)(EEndDate - EStartDate).TotalDays, EEndDate);
                                      ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(EStartDate, EEndDate), EStartDate, EEndDate);
                                      */


                    StaffWtToExcel.GenRepKitchenTmp(EStartDate,  false); 
                    //StaffWtToExcel.GenRepKitchenTmpYear(EStartDate);

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
                else if (args[0] == "AlohaMenuItemsAllUpdate")
                {
                    /**********************************************
                     * Узнать OwnerId по количеству позиций
                    //string strrrr = "Data Source=NewSquare1\\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
                    //string strrrr = "Data Source=sharikb1\\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
                    string strrrr = "Data Source=avrora1\\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
                    System.Data.SqlClient.SqlConnection conn1 = new System.Data.SqlClient.SqlConnection(strrrr);
                    conn1.Open();
                    System.Data.SqlClient.SqlCommand com = new System.Data.SqlClient.SqlCommand("select FK_owner, count(*) as cnt from item group by FK_owner", conn1);
                    System.Data.SqlClient.SqlDataReader rd = com.ExecuteReader();
                    string rrr = "";
                    while (rd.Read())
                    {
                        rrr += $"{rd.GetValue(0).ToString().ToUpper()} = {rd.GetValue(1).ToString()}   ";
                    }
                    conn1.Close();
                    *****************************************************/


                    string CFCConnection1 = "Data Source=NewSquare1\\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
                    string CFCConnection2 = "Data Source=sharikb1\\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
                    //string CFCConnection2 = "Data Source=intersharik1\\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=";
                    string CityOwnerId1 = "9DFF9173-BCB4-4F83-AE77-C1DB74E6776A";
                    string CityOwnerId2 = "2D7CADD1-7698-4FB0-AC8B-432E64AE6909";
                    Tasks.MenuAllItemsUpdaterTask itemsUpdaterTask = new Tasks.MenuAllItemsUpdaterTask();
                    itemsUpdaterTask.Run(CFCConnection2, CityOwnerId2);
                    itemsUpdaterTask.Run(CFCConnection1, CityOwnerId1);
                    //ToDo - отправлять ошибку на сервер
                }
                else if (args[0] == "PahDay")
                {
                    //PahDay 3 nodebug

                    bool isDebug = false;
                    if (args.Length > 2 && args[2].ToLower() == "debug")
                        isDebug = true;

                    DateTime dtS = DateTime.Now.Date.AddDays(-1);
                    int dDeep = 1;
                    DateTime dtE = dtS.AddDays(dDeep);
                    try
                    {
                        if (args.Length > 1)
                        {
                            dDeep = -Convert.ToInt32(args[1]);
                            dtE = dtS.AddDays(dDeep);
                        }
                    }
                    catch {
                        try {
                            var dates = args[1].Split('-');
                            DateTime.TryParseExact(dates[0], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtS);
                            DateTime.TryParseExact(dates[1], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtE);
                        }
                        catch {
                            dtS = DateTime.Now.Date.AddDays(-1);
                            dtE = dtS.AddDays(dDeep);
                        }
                    }

                    AutoCalc.Calculation.InitMode initMode = AutoCalc.Calculation.InitMode.Common;
                    if (args.Length > 2)
                    {
                        if (args[2].ToUpper() == "MOZG")
                            initMode = AutoCalc.Calculation.InitMode.MozgOnly;
                        if (args.Length > 3)
                            if (args[3].ToUpper() == "MOZG")
                                initMode = AutoCalc.Calculation.InitMode.MozgOnly;

                    }

                    //dtE = new DateTime(2022, 06, 01);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //dtS = new DateTime(2022, 06, 18);//new DateTime(2022, 06, 12);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    ////dtE = new DateTime(2022, 05, 02);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    ////dtS = new DateTime(2022, 05, 31);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    //dtE = new DateTime(2022, 04, 01);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //dtE = new DateTime(2022, 03, 17);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    //dtE = new DateTime(2022, 09, 01);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-10-10
                    //dtS = new DateTime(2022, 09, 30);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-10-10
                    //initMode = AutoCalc.Calculation.InitMode.OnlineShopOnly;  //2022-10-10

                    //dtE = new DateTime(2022, 09, 01);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-10-10 для доли списания алкоголя // 2022-10-14 списание по блюдам // 2022-10-18 продажи нап/алко на гостя //2022-10-20 негативные отзывы ИМ
                    //dtS = new DateTime(2022, 10, 19);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-10-10 для доли списания алкоголя // 2022-10-14 списание по блюдам // 2022-10-18 продажи нап/алко на гостя //2022-10-20 негативные отзывы ИМ

                    //dtE = new DateTime(2022, 09, 01);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-10-20 продажи нап/алко на гостя + негативные отзывы ИМ
                    //dtS = new DateTime(2022, 10, 24);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-10-20 продажи нап/алко на гостя + негативные отзывы ИМ

                    //initMode = AutoCalc.Calculation.InitMode.Spis;

                    //dtE = new DateTime(2022, 11, 19);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-11-20 продажи нап/алко на гостя
                    //dtS = new DateTime(2022, 11, 19);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 2022-11-20 продажи нап/алко на гостя

                    //dtE = new DateTime(2022, 11, 01); //негатив
                    //dtS = new DateTime(2022, 12, 02);



                    //Console.WriteLine($"Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}");
                    Utils.ToLog($"НАЧАЛО РАБОТЫ - Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}   РЕЖИМ: {initMode}", true);
                    List<DateTime> months = new List<DateTime>();
                    AutoCalc.Calculation.Init(dtE, initMode);
                    for (DateTime dt = dtS; dt >= dtE; dt = dt.AddDays(-1))
                    {
                        ReportLogger.InitDay(dt);
                        if (isDebug)
                        {
                            if (dt != dtS)
                                AutoCalc.Calculation.DeleteSpis();
                            AutoCalc.Calculation.DayCalc(dt);
                            DateTime dateMonth = new DateTime(dt.Year, dt.Month, 1);
                            if (!months.Contains(dateMonth))
                                months.Add(dateMonth);
                            ReportLogger.CloseDayOk(dt);
                        }
                        else
                        {
                            try
                            {
                                if (dt != dtS)
                                    AutoCalc.Calculation.DeleteSpis();
                                AutoCalc.Calculation.DayCalc(dt);
                                DateTime dateMonth = new DateTime(dt.Year, dt.Month, 1);
                                if (!months.Contains(dateMonth))
                                    months.Add(dateMonth);
                                ReportLogger.CloseDayOk(dt);
                            }
                            catch (Exception ex) {
                                Utils.ToDebugLog($" Error calculation dt:{dt:dd.MM.yyyy}, message:{ex.Message}", true);
                                ReportLogger.MarkErrorDay(dt); 
                            }
                        }
                    }

                    foreach (DateTime dtM in months)
                    {
                        AutoCalc.Calculation.MonthCalc(dtM);
                    }

                    Utils.ToLog($"УСПЕШНО ЗАВЕРШЕНО - Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}", true);
                }
                else if (args[0] == "PahSpisPrevMonth")
                {
                    DateTime dayLastOfPrev = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date.AddDays(-1);

                    //Console.WriteLine($"Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}");
                    Utils.ToLog($"НАЧАЛО РАБОТЫ - Расчет пахаря ТОЛЬКО СПИСАНИЯ ПРОШЛОГО МЕСЯЦА {dayLastOfPrev.ToShortDateString()}", true);

                    List<DateTime> months = new List<DateTime>();
                    AutoCalc.Calculation.InitOnlySpis();

                    AutoCalc.Calculation.DayCalc(dayLastOfPrev);

                    AutoCalc.Calculation.MonthCalc(new DateTime(dayLastOfPrev.Year, dayLastOfPrev.Month, 1));

                    Utils.ToLog($"УСПЕШНО ЗАВЕРШЕНО - Расчет пахаря ТОЛЬКО СПИСАНИЯ ПРОШЛОГО МЕСЯЦА {dayLastOfPrev.ToShortDateString()}", true);
                }
                else if (args[0] == "PahDayDEGUB_FOR_OTHER_DAY")
                {






                    //DateTime dtS = DateTime.Now.Date.AddDays(-1);
                    //int dDeep = 1;
                    //try
                    //{
                    //    if (args.Length > 1)
                    //    {
                    //        dDeep = -Convert.ToInt32(args[1]);

                    //    }
                    //}
                    //catch { }
                    //DateTime dtE = dtS.AddDays(dDeep);

                    //DateTime dtS = new DateTime(2021, 06, 02);
                    //DateTime dtE = new DateTime(2021, 06, 01);

                    List<DateTime> days = new List<DateTime>()
                    {
                        //new DateTime(2021, 06, 18)
                        ///                        
                        
                        new DateTime(2021, 06, 26),
                        new DateTime(2021, 06, 25),
                        new DateTime(2021, 06, 23),
                        new DateTime(2021, 06, 24),
                    };

                    Console.WriteLine($"Расчет пахаря DEGUB 08_06_2021 {days.First().ToShortDateString()} - {days.Last().ToShortDateString()}");

                    AutoCalc.Calculation.Init();
                    foreach (DateTime dt in days)
                    {
                        Console.WriteLine($" Расчет пахаря {dt.ToShortDateString()}");
                        AutoCalc.Calculation.DayCalc(dt);
                    }

                    Console.WriteLine($"УСПЕШНО ЗАВЕРШЕНО - Расчет пахаря DEGUB 08_06_2021 {days.First().ToShortDateString()} - {days.Last().ToShortDateString()}");
                }
                else if (args[0] == "PahDayDEBUG")
                {

                    //       МЕСЯЦ




                    List<DateTime> days1 = new List<DateTime>()
                    {
                        new DateTime(2022, 01, 01),
                        new DateTime(2022, 01, 02),
                        new DateTime(2022, 01, 03),
                        new DateTime(2022, 01, 04),
                        new DateTime(2022, 01, 05),
                        new DateTime(2022, 01, 06),
                        new DateTime(2022, 01, 07),
                        new DateTime(2022, 01, 08),
                        new DateTime(2022, 01, 09),
                        new DateTime(2022, 01, 10),
                        new DateTime(2022, 01, 11),
                        new DateTime(2022, 01, 12),
                        new DateTime(2022, 01, 13),
                        new DateTime(2022, 01, 14),
                        new DateTime(2022, 01, 15),
                        new DateTime(2022, 01, 16),
                        new DateTime(2022, 01, 17),
                        new DateTime(2022, 01, 18),
                        new DateTime(2022, 01, 19),
                        new DateTime(2022, 01, 20),
                        new DateTime(2022, 01, 21),
                        new DateTime(2022, 01, 22),
                        new DateTime(2022, 01, 23),
                        new DateTime(2021, 01, 24),
                        new DateTime(2021, 01, 25),
                        new DateTime(2021, 01, 26),
                        new DateTime(2021, 01, 27),
                        new DateTime(2021, 01, 28),
                        new DateTime(2021, 01, 29),
                        new DateTime(2021, 01, 30),
                        new DateTime(2021, 01, 31),
                        new DateTime(2021, 02, 01),
                        new DateTime(2021, 02, 02),
                        new DateTime(2021, 02, 03),
                    };

                    AutoCalc.Calculation.InitForDebug_08_06_21(); //var typs = new List<int>() { 2, 15, 17, 19, 22, 23 };

                    foreach (DateTime dt in days1)
                    //for (DateTime dt = new DateTime(2021,10,11); dt >= new DateTime(2021,06,05); dt = dt.AddDays(-1))
                    //for (DateTime dt = new DateTime(2021, 10, 11); dt >= new DateTime(2021, 10, 01); dt = dt.AddDays(-1))
                    {
                        Console.WriteLine($" Расчет пахаря {dt.ToShortDateString()}");
                        AutoCalc.Calculation.DayCalc(dt);
                    }

                    //ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(days1.First(), days1.Last()), days1.First(), days1.Last());

                    return;

                    int cntAll = 0;
                    int cntWrongMin = 0;
                    int cntWrongMax = 0;
                    int cntWrongMinIgnored = 0;
                    int cntWrongMinUnited = 0;

                    int dep = 350;// 295;/// 350;// 255; 270;
                    string pref = "SOS";// "ST";//"SOS"
                    List<string> paths = new List<string>();
                    days1.ForEach(_day => paths.Add($"\\\\nas\\winshare\\sos\\{dep}\\{pref}{_day:yyyy}{_day:MM}{_day:dd}.xml"));

                    Dictionary<OrderTimes, string> outResult = new Dictionary<OrderTimes, string>();

                    for (int i = 0; i < paths.Count(); i++)
                    {
                        QSR.QSRTiming.InsertQSRXMLInTable_FOR_DEBUG_ONLY(new List<string>() { paths[i] }, dep, days1[i], outResult);
                    }
                    List<int> KitchenItems = CubeData.GetKitchenDList();
                    outResult = outResult
                        .Where(a => KitchenItems.Contains(a.Key.ItemId.Value)).ToList()
                        .OrderBy(_res => _res.Key.BusinessDate)
                        .ThenBy(_res => _res.Key.TransactionNumber)
                        .ToDictionary(_key => _key.Key, _val => _val.Value);


                    //Повар, су-шеф, старший повар
                    List<int> KPos = new List<int>() { 2, 8, 4 };
                    S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
                    S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

                    //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    //app.Visible = true;
                    //Microsoft.Office.Interop.Excel.Workbook Wb = app.Workbooks.Add(true);
                    //Microsoft.Office.Interop.Excel.Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
                    //Ws.Name = $"Заказы {dep} 1-7дек2021";
                    

                    //Ws.Cells[1, 1] = "Дата";
                    //Ws.Cells[1, 2] = "№ транзакции";
                    //Ws.Cells[1, 3] = "Сервер";
                    //Ws.Cells[1, 4] = "VirtualDisplayId";
                    //Ws.Cells[1, 5] = "№ стола";
                    //Ws.Cells[1, 6] = "Начало заказа";
                    //Ws.Cells[1, 7] = "Конец заказа";
                    //Ws.Cells[1, 8] = "Позиция";
                    //Ws.Cells[1, 9] = "Наименование";
                    //Ws.Cells[1, 10] = "OrderFirstDisplayedTime";
                    //Ws.Cells[1, 11] = "OrderLastBumpTime";
                    //Ws.Cells[1, 12] = "поз. ЭТАЛОН";
                    //Ws.Cells[1, 13] = "поз. факт";
                    //Ws.Cells[1, 14] = "поз. Результ";

                    //Ws.Cells[1, 15] = "поз. Игнор";
                    //Ws.Cells[1, 16] = "поз. Игнор РЕЗ";

                    //Ws.Cells[1, 17] = "поз. Union";
                    //Ws.Cells[1, 18] = "поз. Union РЕЗ";

                    //Ws.Cells[1, 19] = "заказ ЭТАЛОН";
                    //Ws.Cells[1, 20] = "заказ факт1";
                    //Ws.Cells[1, 21] = "заказ факт2";
                    //Ws.Cells[1, 22] = "заказ ИГНОР";
                    //Ws.Cells[1, 23] = "заказ UNION";
                    //Ws.Cells[1, 24] = "заказ Результ1";
                    //Ws.Cells[1, 25] = "заказ Результ2";
                    //Ws.Cells[1, 26] = "заказ Результ Ингор";
                    //Ws.Cells[1, 27] = "заказ Результ Union";

                    int row = 2;
                    int prevTrans = -1;
                    foreach(var keyPair in outResult)
                    {
                        if (!KitchenItems.Contains((int)keyPair.Key.ItemId)) continue;

                        var order = outResult.Where(_res => _res.Key.BusinessDate == keyPair.Key.BusinessDate && _res.Key.TransactionNumber == keyPair.Key.TransactionNumber);

                        //order = order.Where(_obj => !(_obj.Key.OrderFirstDisplayedTime > 0 && order
                        //                    .Any(_itm => _itm.Key.ItemId == _obj.Key.ItemId && _itm.Key.ItemCookTime == _obj.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == _obj.Key.OrderFirstDisplayedTime)));

                        //foreach(var obj in order)
                        //{                           
                        //    if(order.Any(_itm => _itm.Key.ItemId == obj.Key.ItemId && _itm.Key.ItemCookTime == obj.Key.ItemCookTime && _itm.Key.OrderFirstDisplayedTime == obj.Key.OrderLastBumpTime))
                        //    {
                        //        var twin = order.FirstOrDefault(_itm => _itm.Key.ItemId == obj.Key.ItemId && _itm.Key.ItemCookTime == obj.Key.ItemCookTime && _itm.Key.OrderFirstDisplayedTime == obj.Key.OrderLastBumpTime);
                        //        obj.Key.OrderLastBumpTime += (twin.Key.OrderLastBumpTime - twin.Key.OrderFirstDisplayedTime);
                        //        twin.Key.ItemId = 0;
                        //    }
                        //}

                        if (order.Count(_res => _res.Key.ItemId > 0) == 0) 
                            continue;

                        if(keyPair.Key.TransactionNumber != prevTrans)
                        {
                            prevTrans = (int)keyPair.Key.TransactionNumber;
                            row++;
                        }

                        cntAll++;

                        int MaxItemCookTime = order.Select(a => a.Key.ItemCookTime.Value).Max();
                        int MaxPrepTime1 = order.Where(a => a.Key.ItemCookTime == MaxItemCookTime && a.Key.OrderLastBumpTime > 0)
                            .Select(a => a.Key.OrderLastBumpTime.Value - a.Key.OrderFirstDisplayedTime.Value).Min(); //Первый бамп самого длинного блюда
                        int MaxPrepTime2 = order.Where(a => a.Key.ItemCookTime == MaxItemCookTime && a.Key.OrderLastBumpTime > 0)
                            .Select(a => a.Key.OrderLastBumpTime.Value - a.Key.OrderFirstDisplayedTime.Value).Max(); //Первый бамп самого длинного блюда

                        var prepIng = order.Where(a => a.Key.ItemCookTime == MaxItemCookTime && a.Key.OrderLastBumpTime > 0)
                            .Where(_obj => !(_obj.Key.OrderFirstDisplayedTime > 0 && order
                                            .Any(_itm => _itm.Key.ItemId == _obj.Key.ItemId && _itm.Key.ItemCookTime == _obj.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == _obj.Key.OrderFirstDisplayedTime)));

                        //order = order.Where(_obj => !(_obj.Key.OrderFirstDisplayedTime > 0 && order
                        //                    .Any(_itm => _itm.Key.ItemId == _obj.Key.ItemId && _itm.Key.ItemCookTime == _obj.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == _obj.Key.OrderFirstDisplayedTime)));

                        int t1 = order.Count();
                        int t2 = prepIng.Count();

                        int MaxPrepTimeIgored = prepIng.Count() > 0
                            ? prepIng.Select(a => a.Key.OrderLastBumpTime.Value - a.Key.OrderFirstDisplayedTime.Value).Min() //Первый бамп самого длинного блюда ИНГОРИНУЯ БЛЮДА НЕ С НАЧАЛА
                            : 0;
                        var prepUni = order.Where(a => a.Key.ItemCookTime == MaxItemCookTime && a.Key.OrderLastBumpTime > 0 &&
                                 !(a.Key.OrderFirstDisplayedTime == 0 && order
                                 .Any(_itm => _itm.Key.ItemId == a.Key.ItemId && _itm.Key.ItemCookTime == a.Key.ItemCookTime && _itm.Key.OrderFirstDisplayedTime == a.Key.OrderLastBumpTime)));
                        int MaxPrepTimeUnited = prepUni.Count() > 0
                            ? prepUni.Select(a =>
                            {
                                var baseTime = a.Key.OrderLastBumpTime.Value - a.Key.OrderFirstDisplayedTime.Value;
                                if (a.Key.OrderFirstDisplayedTime.Value > 10)
                                {
                                    if (order.Any(_itm => _itm.Key.ItemId == a.Key.ItemId && _itm.Key.ItemCookTime == a.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == a.Key.OrderFirstDisplayedTime))
                                    {
                                        var twinItem = order.First(_itm => _itm.Key.ItemId == a.Key.ItemId && _itm.Key.ItemCookTime == a.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == a.Key.OrderFirstDisplayedTime);
                                        baseTime += (twinItem.Key.OrderLastBumpTime.Value - twinItem.Key.OrderFirstDisplayedTime.Value);
                                    }
                                    //var twinItem = order.FirstOrDefault(_itm => _itm.Key.ItemId == a.Key.ItemId && _itm.Key.ItemCookTime == a.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == a.Key.OrderFirstDisplayedTime);
                                    //if (twinItem != null)
                                    //    baseTime += (twinItem.Key.OrderLastBumpTime.Value - twinItem.Key.OrderFirstDisplayedTime.Value);
                                };
                                return baseTime;
                            }).Min()
                            : 0; //Первый бамп самого длинного блюда ИНГОРИНУЯ БЛЮДА НЕ С НАЧАЛА

                        int thisUnion = (int)(keyPair.Key.OrderLastBumpTime - keyPair.Key.OrderFirstDisplayedTime);
                        if (keyPair.Key.OrderFirstDisplayedTime.Value > 10)
                        {
                            if (order.Any(_itm => _itm.Key.ItemId == keyPair.Key.ItemId && _itm.Key.ItemCookTime == keyPair.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == keyPair.Key.OrderFirstDisplayedTime))
                            {
                                var twinItem = order.First(_itm => _itm.Key.ItemId == keyPair.Key.ItemId && _itm.Key.ItemCookTime == keyPair.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == keyPair.Key.OrderFirstDisplayedTime);
                                thisUnion += (twinItem.Key.OrderLastBumpTime.Value - twinItem.Key.OrderFirstDisplayedTime.Value);
                            }
                            //var twinItem = order.FirstOrDefault(_itm => _itm.Key.ItemId == a.Key.ItemId && _itm.Key.ItemCookTime == a.Key.ItemCookTime && _itm.Key.OrderLastBumpTime == a.Key.OrderFirstDisplayedTime.Value);
                            //if (twinItem != null)
                            //    baseTime += (twinItem.Key.OrderLastBumpTime.Value - twinItem.Key.OrderFirstDisplayedTime.Value);
                        };
                        int thisIgnored = (int)(keyPair.Key.OrderFirstDisplayedTime  < 10 ? (keyPair.Key.OrderLastBumpTime - keyPair.Key.OrderFirstDisplayedTime) : 0);

                        //Ws.Cells[row, 1] = keyPair.Key.BusinessDate;
                        //Ws.Cells[row, 2] = keyPair.Key.TransactionNumber;
                        //Ws.Cells[row, 3] = keyPair.Key.ServerId;
                        //Ws.Cells[row, 4] = keyPair.Key.VirtualDisplayId;
                        //Ws.Cells[row, 5] = keyPair.Key.TableNum;
                        //Ws.Cells[row, 6] = keyPair.Key.OrderStartTime;
                        //Ws.Cells[row, 7] = keyPair.Key.OrderEndTime;
                        //Ws.Cells[row, 8] = keyPair.Key.ItemId;
                        //Ws.Cells[row, 9] = keyPair.Value;
                        //Ws.Cells[row, 10] = keyPair.Key.OrderFirstDisplayedTime;
                        //Ws.Cells[row, 11] = keyPair.Key.OrderLastBumpTime;
                        //Ws.Cells[row, 12] = keyPair.Key.ItemCookTime;
                        //if (keyPair.Key.ItemCookTime == MaxItemCookTime)
                        //    Ws.Cells[row, 12].Font.Bold = true;
                        //Ws.Cells[row, 13] = (keyPair.Key.OrderLastBumpTime - keyPair.Key.OrderFirstDisplayedTime);
                        //if (keyPair.Key.ItemCookTime == MaxItemCookTime && keyPair.Key.OrderLastBumpTime > 0 && ((keyPair.Key.OrderLastBumpTime - keyPair.Key.OrderFirstDisplayedTime) == MaxPrepTime1))
                        //    Ws.Cells[row, 13].Font.Bold = true;
                        //if (keyPair.Key.ItemCookTime == MaxItemCookTime && keyPair.Key.OrderLastBumpTime > 0 && ((keyPair.Key.OrderLastBumpTime - keyPair.Key.OrderFirstDisplayedTime) == MaxPrepTime2))
                        //    Ws.Cells[row, 13].Font.Strikethrough = true;
                        //Ws.Cells[row, 14] = ((keyPair.Key.OrderLastBumpTime - keyPair.Key.OrderFirstDisplayedTime) > keyPair.Key.ItemCookTime) ? "просроч." : "";

                        //Ws.Cells[row, 15] = thisIgnored;
                        //Ws.Cells[row, 15].Font.Strikethrough = true;
                        //Ws.Cells[row, 15].Font.Italic = true;
                        //Ws.Cells[row, 16] = ((thisIgnored) > keyPair.Key.ItemCookTime) ? "просроч." : "";

                        //Ws.Cells[row, 17] = thisUnion;
                        //Ws.Cells[row, 17].Font.Strikethrough = true;
                        //Ws.Cells[row, 17].Font.Bold = true;
                        //Ws.Cells[row, 18] = ((thisUnion) > keyPair.Key.ItemCookTime) ? "просроч." : "";

                        //Ws.Cells[row, 19] = MaxItemCookTime;
                        //Ws.Cells[row, 20] = MaxPrepTime1;
                        //Ws.Cells[row, 21] = MaxPrepTime2;
                        //Ws.Cells[row, 22] = MaxPrepTimeIgored;
                        //Ws.Cells[row, 23] = MaxPrepTimeUnited;
                        //Ws.Cells[row, 24] = (MaxPrepTime1 > MaxItemCookTime) ? "ПРОСРОЧ" : "";
                        //Ws.Cells[row, 24].Font.Bold = true;
                        //Ws.Cells[row, 25] = (MaxPrepTime2 > MaxItemCookTime) ? "ПРОСРОЧ" : "";
                        //Ws.Cells[row, 25].Font.Strikethrough = true;
                        //Ws.Cells[row, 26] = (MaxPrepTimeIgored > MaxItemCookTime) ? "ПРОСРОЧ" : "";
                        //Ws.Cells[row, 26].Font.Strikethrough = true;
                        //Ws.Cells[row, 26].Font.Italic = true;
                        //Ws.Cells[row, 27] = (MaxPrepTimeUnited > MaxItemCookTime) ? "ПРОСРОЧ" : "";
                        //Ws.Cells[row, 27].Font.Strikethrough = true;
                        //Ws.Cells[row, 27].Font.Bold = true;
                        ////Ws.Cells[row, 13] = keyPair.Key.Id;

                        cntWrongMin += (MaxPrepTime1 > MaxItemCookTime) ? 1 : 0;
                        cntWrongMax += (MaxPrepTime2 > MaxItemCookTime) ? 1 : 0;
                        cntWrongMinIgnored += (MaxPrepTimeIgored > MaxItemCookTime) ? 1 : 0;
                        cntWrongMinUnited += (MaxPrepTimeUnited > MaxItemCookTime) ? 1 : 0;

                        row++;
                    }


                    //row++;
                    //Ws.Cells[row, 2] = "Всего";
                    //Ws.Cells[row, 3] = cntAll;
                    //row++;
                    //Ws.Cells[row, 2] = "Min";
                    //Ws.Cells[row, 3] = cntWrongMin;
                    //Ws.Cells[row, 4] = (double)cntWrongMin / (double)cntAll;
                    //row++;
                    //Ws.Cells[row, 2] = "Max";
                    //Ws.Cells[row, 3] = cntWrongMax;
                    //Ws.Cells[row, 4] = (double)cntWrongMax / (double)cntAll;
                    //row++;
                    //Ws.Cells[row, 2] = "Ignore";
                    //Ws.Cells[row, 3] = cntWrongMinIgnored;
                    //Ws.Cells[row, 4] = (double)cntWrongMinIgnored / (double)cntAll;
                    //row++;
                    //Ws.Cells[row, 2] = "United";
                    //Ws.Cells[row, 3] = cntWrongMinUnited;
                    //Ws.Cells[row, 4] = (double)cntWrongMinUnited / (double)cntAll;

                    var Min = (double)cntWrongMin / (double)cntAll;
                    var Max = (double)cntWrongMax / (double)cntAll;
                    var Ing = (double)cntWrongMinIgnored / (double)cntAll;
                    var Uni = (double)cntWrongMinUnited / (double)cntAll;

                    return;

                    //AutoCalc.Calculation.InitOnlySpis();
                    //AutoCalc.Calculation.Init();
                    AutoCalc.Calculation.InitForDebug_08_06_21(); //var typs = new List<int>() { 2, 15, 17, 19, 22, 23 };




                    foreach (DateTime dt in days1)
                    //for (DateTime dt = new DateTime(2021,10,11); dt >= new DateTime(2021,06,05); dt = dt.AddDays(-1))
                    //for (DateTime dt = new DateTime(2021, 10, 11); dt >= new DateTime(2021, 10, 01); dt = dt.AddDays(-1))
                    {
                        Console.WriteLine($" Расчет пахаря {dt.ToShortDateString()}");
                        AutoCalc.Calculation.DayCalc(dt);
                    }


                    ////AutoCalc.Calculation.Init();
                    ////AutoCalc.Calculation.MonthCalc(new DateTime(2021, 08, 01));
                    ////AutoCalc.Calculation.MonthCalc(new DateTime(2021, 07, 01));

                    var ttt1 = TimeOfPreparation.test;
                    var ttt2 = TimeOfPreparation.testStrange;
                    var ttt3 = TimeOfPreparation.testbumps;

                    //ReportTableUpdater.OrderTimeBySuSheffOrders(TimeOfPreparation.GetTimeOfPrepOfOrdersByDayAndSuShef(days1.Last(), days1.First().AddDays(1)), days1.Last(), days1.First().AddDays(1));
                    return;

                    return;

                    //DateTime dtS = DateTime.Now.Date.AddDays(-1);
                    //int dDeep = -3;
                    //try
                    //{
                    //    if (args.Length > 1)
                    //    {
                    //        dDeep = -Convert.ToInt32(args[1]);

                    //    }
                    //}
                    //catch { }
                    //DateTime dtE = dtS.AddDays(dDeep);
                    ////Console.WriteLine($"Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}");
                    //Utils.ToLog($"ТЕСТ - Расчет пахаря {dtS.ToShortDateString()} - {dtE.ToShortDateString()}", true);

                    //List<DateTime> months = new List<DateTime>();
                    //AutoCalc.Calculation.Init();
                    //for (DateTime dt = dtS; dt >= dtE; dt = dt.AddDays(-1))
                    //{
                    //    DateTime dateMonth = new DateTime(dt.Year, dt.Month, 1);
                    //    if (!months.Contains(dateMonth))
                    //        months.Add(dateMonth);
                    //}

                    //foreach (DateTime dtM in months)
                    //{
                    //    AutoCalc.Calculation.MonthCalc(dtM);
                    //}



                    //return;


                    ////int dDeep = -5;
                    ////DateTime EEndDate = DateTime.Now.Date;
                    ////DateTime EStartDate = EEndDate.AddDays(dDeep);




                    List<DateTime> days = new List<DateTime>()
                    {                      
                       // new DateTime(2021, 08, 03),
                        new DateTime(2021, 07, 31),

                    };

                    Console.WriteLine($"Расчет пахаря DEGUB 08_06_2021 {days.First().ToShortDateString()} - {days.Last().ToShortDateString()}");

                AutoCalc.Calculation.InitForDebug_08_06_21();
                foreach (DateTime dt in days)
                    {
                        Console.WriteLine($" Расчет пахаря {dt.ToShortDateString()}");
                        AutoCalc.Calculation.DayCalc(dt);
                }

                Console.WriteLine($"УСПЕШНО ЗАВЕРШЕНО - Расчет пахаря DEGUB 08_06_2021 {days.First().ToShortDateString()} - {days.Last().ToShortDateString()}");
                }
                else if (args[0] == "Dev")
                {
                    //DateTime Fday = new DateTime(2021, 06, 15);
                    //DateTime Eday = Fday.AddDays(-1);

                    //var DrinksListCup = CubeData.GetDishOfCat(92); //напитки в бокалах
                    //var DrinksCupCount = AutoCalc.CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(Fday, string.Join(",", DrinksListCup.ToArray()), Day: Fday.Day));

                    ////Кол-во напитков в бутылках
                    //var DrinksListBottle = CubeData.GetDishOfCat(91); //напитки в бутылках
                    //var DrinksBottleCount = AutoCalc.CorrectTwinDep.Correct(CubeData.GetDishesCountNoDelevery(Fday, string.Join(",", DrinksListBottle.ToArray()), Day: Fday.Day));




                    return;
#region Dev
                    DateTime Fdt = new DateTime(2021, 06, 15);
                    DateTime Edt = Fdt.AddDays(-1);

                    {
                        ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient servicesObjClient2 = new ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient();

                        vfiliasut8.ExchangePeskovFotoGallery peskovFotoGallery1 = new vfiliasut8.ExchangePeskovFotoGallery();
                        System.Net.NetworkCredential networkCredential1 = new System.Net.NetworkCredential("ws", "ws1", "");
                        peskovFotoGallery1.Credentials = (System.Net.ICredentials)networkCredential1;
                        int number = 371;
                        int? cod_virt_shop;
                        servicesObjClient2.obj_virt(new int?(number), out cod_virt_shop);
                        number = cod_virt_shop.Value;

                        double numCosts = 0.0;
                        try
                        {
                            numCosts = peskovFotoGallery1.getCosts(number.ToString(), Edt, Fdt);

                        }
                        catch (Exception ex)
                        {
                            ;
                        }
                    }
                    return;


                    int Dep = 310;

                    List<DateTime> days = new List<DateTime>()
                    {
                        ////new DateTime(2021, 06, 2)
                        //new DateTime(2021, 06, 15),
                        //new DateTime(2021, 06, 14),
                        //new DateTime(2021, 06, 13),
                        //new DateTime(2021, 05, 10),
                        //new DateTime(2021, 05, 9),
                        //new DateTime(2021, 05, 8),
                        //new DateTime(2021, 04, 9),
                        new DateTime(2021, 04, 8),
                        new DateTime(2021, 04, 7),
                        new DateTime(2021, 04, 6),
                        new DateTime(2021, 04, 5),
                        new DateTime(2021, 03, 7),
                        new DateTime(2021, 03, 6),
                        new DateTime(2021, 03, 5),
                    };

                    //for (int i = 1; i <= 5; i++)
                    //{
                    //    days.Add(days[0].AddDays(-i));
                    //}

                    vfiliasut8.ExchangePeskovFotoGallery peskovFotoGallery = new vfiliasut8.ExchangePeskovFotoGallery();
                    System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential("ws", "ws1", "");
                    peskovFotoGallery.Credentials = (System.Net.ICredentials)networkCredential;

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                    {
                        sw.WriteLine();
                        sw.WriteLine(" *************************************** " + DateTime.Now+" *************************************** ");
                    }
                    foreach (DateTime day in days)
                    {
                        Edt = day;
                        Fdt = Edt.AddDays(-1); ;// new DateTime(2021, 06, 14);
                        //Fdt = day;// new DateTime(2021, 06, 14);
                        //Edt = Fdt.AddDays(-1);


                        Dictionary<int, int> test = new Dictionary<int, int>();

                        string str;

                        ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient servicesObjClient2 = new ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient();
                        S2010.DepartmentInfo[] pointList3 = new S2010.XrepSoapClient().GetPointList3();


                        { 

                        Dictionary<int, List<int>> unionsOfPord = new Dictionary<int, List<int>>();

                        Dictionary<int, int> buhNums = new Dictionary<int, int>();
                        //Dictionary<int, double> buhCash = new Dictionary<int, double>();
                        //Dictionary<int, double> buhCashMinNDS = new Dictionary<int, double>();
                        Dictionary<int, double> buhCashCosts = new Dictionary<int, double>();

                        foreach (S2010.DepartmentInfo departmentInfo in pointList3)
                                if(departmentInfo.Enabled)
                                    if(departmentInfo.Number == 104)
                        {
                            int number = departmentInfo.Number;
                            //if (departmentInfo.Number != 200 && departmentInfo.Number != 310)
                            {
                                int? cod_virt_shop;
                                servicesObjClient2.obj_virt(new int?(departmentInfo.Number), out cod_virt_shop);
                                number = cod_virt_shop.Value;
                                if (!buhNums.ContainsKey(departmentInfo.Number))
                                    buhNums.Add(departmentInfo.Number, number);

                                double numCosts = 0.0;
                                try
                                {
                                    numCosts = peskovFotoGallery.getCosts(number.ToString(), Fdt, Edt);

                                    if (!buhCashCosts.ContainsKey(departmentInfo.Number))
                                        buhCashCosts.Add(departmentInfo.Number, 0);
                                    buhCashCosts[departmentInfo.Number] += numCosts;
                                    //if (!buhCashCosts.ContainsKey(number))
                                    //    buhCashCosts.Add(number, 0);
                                    //buhCashCosts[number] += numCosts;
                                }
                                catch (Exception ex)
                                {
                                    ;
                                }
                                        double proceeds = 0;

                                        Ges3.GestoriCashByDay_T_cashRow[] cash;
                                        servicesObjClient2.GestoriCashByDay(Edt, false,
                                            new Ges3.GestoriCashByDay_T_shopsRow[] { new Ges3.GestoriCashByDay_T_shopsRow() { codShop = departmentInfo.Number } },
                                            out cash);

                                        var proceedsTTTTEEEEEESSSSSSSSSSTTTTTTT = (double)(cash[0].sum_nal + cash[0].sum_plast);

                                        proceeds = cash.Select(_cash => (double)(_cash.sum_nal + _cash.sum_plast)).Sum();

                                        str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Podr: {departmentInfo.Number}   costs: {numCosts}   proceed: {proceeds}";
                                        Console.WriteLine(str);
                                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                                            sw.WriteLine(str);
                                    }
                                }

                        //List<int> Unions = new List<int>() { 100};// { 0, 10, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
                        //foreach (int Code_UNION in Unions)
                        //{
                        //    WebSrvSpisanie.Ges3ServicesUTF8ObjClient ClUTF8 = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();

                        //    WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[] T_salesRow;

                        //    ClUTF8.get_cash_sales_by_code(Fdt, Edt, Code_UNION, out T_salesRow);

                        //    var query = T_salesRow.GroupBy(
                        //        _row => _row.cod_podr,
                        //        _row => _row.TOVsum,
                        //        (cod_podr, TOVsum) => new
                        //        {
                        //            Key = cod_podr,
                        //            Sum = TOVsum.Sum(),
                        //            Count = TOVsum.Count(),
                        //            AVGCheck = TOVsum.Count() > 0 ? (TOVsum.Sum() / TOVsum.Count()) : 0
                        //        });

                        //    ////for (int i = 0; i < T_salesRow.Count(); i++)
                        //    ////    if (T_salesRow[i].cod_podr != null)
                        //    ////    {
                        //    ////        int podrBuh = (int)T_salesRow[i].cod_podr;
                        //    ////        if (!buhCash.ContainsKey(podrBuh))
                        //    ////            buhCash.Add(podrBuh, 0);
                        //    ////        buhCash[podrBuh] += (double)T_salesRow[i].TOVsum;

                        //    ////        if (!buhCashMinNDS.ContainsKey(podrBuh))
                        //    ////            buhCashMinNDS.Add(podrBuh, 0);
                        //    ////        buhCashMinNDS[podrBuh] += ((double)T_salesRow[i].TOVsum - (double)T_salesRow[i].NDSsum);


                        //    ////        if (!unionsOfPord.ContainsKey(podrBuh))
                        //    ////            unionsOfPord.Add(podrBuh, new List<int>());
                        //    ////        if (!unionsOfPord[podrBuh].Contains(Code_UNION))
                        //    ////            unionsOfPord[podrBuh].Add(Code_UNION);
                        //    ////    }

                        //    //if (T_salesRow.Count() > 0)
                        //    //{
                        //    //    str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Union: {Code_UNION}    Pords: " + string.Join(", ", T_salesRow.ToList().Select(_tS => _tS.cod_podr).Distinct());
                        //    //    Console.WriteLine(str);
                        //    //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //    //        sw.WriteLine(str);
                        //    //}
                        //}

                        //WebSrvSpisanie.Ges3ServicesUTF8ObjClient ClUTF8 = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();

                        //WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[] T_salesRow;

                        //ClUTF8.get_cash_sales_by_code(Edt/*Fdt*/, Edt, 100, out T_salesRow);

                        //var SumForRest = T_salesRow.GroupBy(
                        //    _row => _row.cod_podr,
                        //    _row => _row.TOVsum,
                        //    (cod_podr, TOVsum) => new
                        //    {
                        //        CodPodr = cod_podr != null ? (int)cod_podr : 0,
                        //        Sum = TOVsum.Sum(),
                        //        SumNoNDS = TOVsum.Sum() - T_salesRow.Where(_row => _row.cod_podr == cod_podr).Sum(_row => _row.NDSsum),
                        //        Count = TOVsum.Count(),
                        //        AVGCheck = TOVsum.Count() > 0 ? (TOVsum.Sum() / TOVsum.Count()) : 0
                        //    });


                        //foreach (KeyValuePair<int, List<int>> keyPair in unionsOfPord)
                        //{
                        //    str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Unions Of Pord: {keyPair.Key}      Unions: " + string.Join(", ", keyPair.Value);
                        //    Console.WriteLine(str);
                        //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //        sw.WriteLine(str);
                        //}

                        //foreach (KeyValuePair<int, double> keyPair in buhCash)
                        //{
                        //    double costs = buhCashCosts.ContainsKey(keyPair.Key) ? buhCashCosts[keyPair.Key] : 0;
                        //    str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Podr: {keyPair.Key}        Costs:   {costs}        TOVsum:   {keyPair.Value}        TOVsumNoNDS:   "+(buhCashMinNDS.ContainsKey(keyPair.Key)? buhCashMinNDS[keyPair.Key].ToString():"-");
                        //    Console.WriteLine(str);
                        //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //        sw.WriteLine(str);
                        //}

                        //foreach (var sum in SumForRest)
                        //{
                        //    double costs = buhCashCosts.ContainsKey(sum.CodPodr) ? buhCashCosts[sum.CodPodr] : 0;
                        //        //str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Podr: {sum.CodPodr}        Costs:   {costs}        TOVsum:   {sum.Sum}        TOVNoNDSsum:   {sum.SumNoNDS}        Count:   {sum.Count}        AVGCheck:   {sum.AVGCheck}";
                        //        str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Podr: {sum.CodPodr}        Costs:   {costs}        TOVsum:   {sum.Sum}        Count:   {sum.Count}        AVGCheck:   {sum.AVGCheck}";
                        //        Console.WriteLine(str);
                        //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //        sw.WriteLine(str);
                        //}


                        //List<int> numsExists = buhNums.Select(_buhNum => _buhNum.Key).ToList();
                        //List<int> noExPodrAll = pointList3.Select(_podr => _podr.Number).Where(_num => !numsExists.Contains(_num)).ToList();
                        //List<int> noExPodrEna = pointList3.Where(_podr => _podr.Enabled).Select(_podr => _podr.Number).Where(_num => !numsExists.Contains(_num)).ToList();

                        //str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Охваченные подразделения: " + string.Join(", ", numsExists);
                        //Console.WriteLine(str);
                        //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //    sw.WriteLine(str);

                        //str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Неохваченные подразделения (все): " + string.Join(", ", noExPodrAll);
                        //Console.WriteLine(str);
                        //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //    sw.WriteLine(str);

                        //str = $"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd}   Неохваченные подразделения (только Enabled): " + string.Join(", ", noExPodrEna);
                        //Console.WriteLine(str);
                        //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                        //    sw.WriteLine(str);
                    }
                        if (false)
                        foreach (S2010.DepartmentInfo departmentInfo in pointList3)
                    {
                        if (departmentInfo.Enabled)
                        {
                            ReportMonthResultGenerator.TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] timeTGoodtimeRowArray = new ReportMonthResultGenerator.TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                            int number = departmentInfo.Number;
                            //ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient servicesObjClient2 = new ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient();

                            //if (departmentInfo.Number != 200 && departmentInfo.Number != 310)
                            {
                                int? cod_virt_shop;
                                servicesObjClient2.obj_virt(new int?(departmentInfo.Number), out cod_virt_shop);
                                number = cod_virt_shop.Value;
                                test.Add(departmentInfo.Number, number);
                            }



                                WebSrvSpisanie.Ges3ServicesUTF8ObjClient ClUTF8 = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();

                                WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[] T_salesRow;// = new WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[0];
                                WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[] T_salesRowBuh;

                                var t1 = ClUTF8.get_cash_sales_by_code(Fdt, Edt, departmentInfo.Number, out T_salesRow);
                                var t2 = ClUTF8.get_cash_sales_by_code(Fdt, Edt, number, out T_salesRowBuh);
                                
                                if (T_salesRow.Count() != 0 || T_salesRowBuh.Count()!=0)
                                {
                                    string distStr = string.Join(",", T_salesRow.Select(_t => _t.cod_podr).Distinct())
                                            + "   "
                                            + string.Join(",", T_salesRowBuh.Select(_t => _t.cod_podr).Distinct());

                                    Console.WriteLine($"BINGO !!!!!!!    {Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd} {departmentInfo.Number} {T_salesRow.Count()}/{T_salesRowBuh.Count()}   {distStr}");

                                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums_Bingo.txt", true))
                                        sw.WriteLine($"BINGO !!!!!!!    {Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd} {departmentInfo.Number} {T_salesRow.Count()}/{T_salesRowBuh.Count()}   {distStr}");
                                }

                                Console.WriteLine($"{Fdt:yyyy.MM.dd} {Edt:yyyy.MM.dd} {departmentInfo.Number} {T_salesRow.Count()}/{T_salesRowBuh.Count()}");
                            }
                    }
                    //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("d:\\buhNums.txt", false))
                    //    sw.Write(string.Join("\n", test.OrderBy(_kk => _kk.Key).Select(_k => 
                    //    {
                    //        string name = pointList3.First(_dep => _dep.Number == _k.Key).Name;
                    //        return $"{_k.Key}\t{name}\t{_k.Value}";
                    //    })));

                    }
                    return;

                    

                    ReportMonthResult DesSpis = new ReportMonthResult();

                    List<int> Deserts = DesertsOnStop.GetDesertList(Dep);

                    WebSrvSpisanie.Ges3ServicesUTF8ObjClient Cll = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();
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
                        Cll.PrihAndSpis(Bc.ToString(), Fdt, Edt, Dep, out cGood, out cMeas, out qPr, out qSp);
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
                        row++;
                    }








                    return;


                    WebSrvSpisanie.Ges3ServicesUTF8ObjClient Cl = new WebSrvSpisanie.Ges3ServicesUTF8ObjClient();

                    WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[] T_sales = new WebSrvSpisanie.get_cash_sales_by_code_T_salesRow[0];
                    
                    Cl.get_cash_sales_by_code(Fdt, Edt, 310, out T_sales);
                    ;
                    ;
                    ;

#endregion Dev
                }
                else if (args[0] == "Detalization3and7")
                {
                    #region Детализация 3 и 7 задачи

                    //Ges3.Ges3ServicesObjClient cl = new Ges3.Ges3ServicesObjClient();

                    //{
                    //    int? r;
                    //    cl.obj_virt(370, out r);
                    //}
                    //return;


                    List<DateTime> days = new List<DateTime>()
                    {
                        new DateTime(2021, 06, 14)
                        //new DateTime(2021, 01, 5),
                        //new DateTime(2021, 02, 5),
                        //new DateTime(2021, 03, 5),
                        //new DateTime(2021, 04, 5),
                        //new DateTime(2021, 05, 5),
                        //new DateTime(2021, 06, 5),
                        //new DateTime(2021, 06, 6),
                        //new DateTime(2021, 06, 7),
                        //new DateTime(2021, 06, 8),
                        //new DateTime(2021, 06, 9),
                        //new DateTime(2021, 06, 10),
                    };

                    for (int i = 1; i <= 1; i++)
                    {
                        ;// days.Add(days[0].AddDays(-i));
                    }

                    System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\ReportGet_Detalization.txt", true);
                    sw.WriteLine(string.Format("{0:dd.MM.yyyy hh:mm:ss}       RepGenStart   детализация за {1:dd.MM.yyyy}", DateTime.Now, string.Join(", ", days.Select(_day => _day.ToShortDateString()))));
                    sw.WriteLine();





                    double limitSecs = 300;
                    var resOut = new List<ReportDayResult>();
                    S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
                    S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

                    //////DepList = DepList.Where(_dep => _dep.Number == 205).ToArray();//////////////////////////////////

                    
                    foreach(DateTime day in days)
                    {

                        //DateTime day = new DateTime(2021, 05, 10);

                        Console.WriteLine($"Детализация времени приготовления {day.ToShortDateString()} - {day.ToShortDateString()}");


                        //var orderTimes = TimeOfPreparation.GetTimeOfPrepOrder(day, day.AddDays(1), false);
                        //var orderTimesWODelivery = TimeOfPreparation.GetTimeOfPrepOrder(day, day.AddDays(1), true);


                        DateTime Fdt = day;
                        DateTime Edt = day.AddDays(1);

                        sw.WriteLine();
                        sw.WriteLine(string.Format("TimeOfPrep за {0} ({1}):", day.ToShortDateString(), day.DayOfWeek));

                        sw.WriteLine(string.Format("№\tНазвание\tКол-во общ.\tКол-во б/дост.\t%"));
                        
                        //if(false)
                        foreach (S2010.DepartmentInfo Dii in DepList)
                        {
                            Console.WriteLine($"\tУчасток   {Dii.Number}   {Dii.Name}");
                            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();

                            // ToDo - это хард, диапазоны столов доставки надо брать из базы
                            List<TimeOfPreparation.Range> ExcludeTables = new List<TimeOfPreparation.Range>() { new TimeOfPreparation.Range(146, 254), new TimeOfPreparation.Range(900, 929) };

                            List<OrderTimes> orderTimes = TimeOfPreparation.GetOrdersOfDepAndDate(Fdt, Edt, Dii.Number, KitchenItems, null);

                            List<OrderTimes> orderTimesWOd = TimeOfPreparation.GetOrdersOfDepAndDate(Fdt, Edt, Dii.Number, KitchenItems, ExcludeTables);

                            int chCount = orderTimes.Count();

                            int chCountWO5t = orderTimesWOd.Count();

                            int badCount = orderTimes.Where(a => a.TableNum == null || a.TableNum == 0).Count();

                            if (Dii.Enabled && chCount > 0)
                                sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                                            Dii.Number,
                                            Dii.Name,
                                            //badCount,
                                            chCount,
                                            chCountWO5t,
                                            chCount != 0 ? (double)((double)chCountWO5t / (double)chCount) : (double)0));
                            ;
                        }












                        sw.WriteLine();





                        Console.WriteLine($"Детализация скорости движения очереди {day.ToShortDateString()} - {day.ToShortDateString()}");

                        sw.WriteLine();

                        sw.WriteLine(string.Format("ShiftSpeesCals за {0} ({1}):", day.ToShortDateString(), day.DayOfWeek));

                        sw.WriteLine(string.Format("№\tНазвание\tКол-во общ.\tКол-во б/5 терм.\t%"));


                        //////if (false)
                        foreach (S2010.DepartmentInfo Dii in DepList.Where(a => a.Place == "Домодедово").OrderBy(a => a.Name))
                        {
                            try
                            {

                                Console.WriteLine($"\tУчасток   {Dii.Number}   {Dii.Name}");

                                var chks = AirReports.GetQSChecks(Dii.Number, day, day.AddDays(1));
                                // Отсеить терминал №5 (его признак - чек начинается с 5-ки)

                                var chksWO5t = chks.Where(_chk => _chk.CheckNum == 0 || int.Parse(_chk.CheckNum.ToString("D7").Substring(0, 3)) != 5).ToList();

                                int chCount = chks.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Count();
                                double tCount = chks.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Sum(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds);
                                double chVal = chCount != 0 ? tCount / chCount : 0;

                                int chCountWO5t = chksWO5t.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Count();
                                double tCountWO5t = chksWO5t.Where(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Sum(a => (a.TClose - a.FirstDishOpenTime).TotalSeconds);
                                double chValWO5t = chCountWO5t != 0 ? tCountWO5t / chCountWO5t : 0;

                                int badCount = chks.Where(a => a.CheckNum <= 0 && (a.TClose - a.FirstDishOpenTime).TotalSeconds < limitSecs && (a.TClose - a.FirstDishOpenTime).TotalSeconds > 0).Count();

                                if (Dii.Number == 222)
                                {
                                    ;
                                }

                                if (Dii.Enabled && chCount > 0)
                                    sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                                                Dii.Number,
                                                Dii.Name,
                                                //badCount,
                                                chCount,
                                                chCountWO5t,
                                                //tCount,
                                                //tCountWO5t,
                                                //chVal,
                                                //chValWO5t,
                                                chCount != 0 ? (double)((double)chCountWO5t / (double)chCount) : (double)0));


                            }
                            catch (Exception e)
                            {
                                ;
                            }
                        }



                    }



                    sw.WriteLine();
                    sw.WriteLine("Расчет детализации окончен");
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();










                    ;
                    sw.Close();
                    #endregion Детализация 3 и 7 задачи
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
