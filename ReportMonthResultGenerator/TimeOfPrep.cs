using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;



namespace ReportMonthResultGenerator
{
   static class TimeOfPreparation
    {


       internal static void ReportGenerate(DateTime Fdt, DateTime Edt)
       {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
           app.Visible = true;
           
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            List<ReportDayQSRTime> Data = GetTimeOfPrepByDayAndSuShef(Fdt, Edt);
           int col =2;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a =>a.Name))
            {
              if (!Dii.Enabled) continue;
                Ws.Cells[ col,1] = Dii.Name;
                int WrCount = Data.Where(a => a.Department.Value == Dii.Number).Sum(a => a.WrongCount).Value;
                int AllCount = Data.Where(a => a.Department.Value == Dii.Number).Sum(a => a.OrdersCount).Value;
                double res = 0;
                if (WrCount > 0)
                {
                    res = (double)WrCount / (double)AllCount;
                }
                Ws.Cells[col, 2] = res;
                col++;
            }
       }


       internal static List<ReportDayQSRTime> GetTimeOfPrepByDayAndSuShef(DateTime Fdt, DateTime Edt)
       {

           Utils.ToLog(String.Format("Запуск GetTimeOfPrepByDayAndSuShef dt1 = {0}, dt2={1} ", Fdt, Edt));
           List<ReportDayQSRTime> Tmp = new List<ReportDayQSRTime>();
           List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetSuShefs(Fdt, Edt), Fdt, Edt);
           S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
           S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
           List<int> KitchenItems = GetKitchenItems();
           TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

            foreach (S2010.DepartmentInfo Dii in DepList)
               {
                   if (!Dii.Enabled) continue;
                   //if (Dii.Number != 350) continue;
                   Console.WriteLine(Dii.Name);
                   Utils.ToLog(String.Format("GetTimeOfPrepByDayAndSuShef Начало расчета подразделение  {0} {1}", Dii.Number, Dii.Name));
                   TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[1];

                   int? kol = 0;
                   PrepSrv.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                   PrepSrv.ShopsGoodTime(Dii.Number, Fdt, Edt.AddDays(-1), out kol, out res);
                   /*
                   PrepTime Pt = new PrepTime()
                   {
                       Dep = Dii.Number,
                       DepName = Dii.Name

                   };
                   */
                   

                   //foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res.Where((a => Convert.ToDateTime(a.OrderTime) > dt && Convert.ToDateTime(a.OrderTime) < dt.AddDays(1))))
                       foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res)
                   {
                       if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                       if (!KitchenItems.Contains(Convert.ToInt32(r.BarCode))) continue;

                       ReportDayQSRTime RecTmp = new ReportDayQSRTime();

                       RecTmp.OrdersCount = 1;
                       RecTmp.AllTime = r.Fact;
                       RecTmp.WrongCount = 0;
                       RecTmp.WrongTime = 0;
                       RecTmp.FactSummOfWrong = 0;
                       RecTmp.NormaSummOfWrong = 0;
                       if (r.Fact > r.Norma)
                       {
                           RecTmp.WrongCount = 1;
                           RecTmp.WrongTime = r.Fact.Value - r.Norma.Value;
                           RecTmp.FactSummOfWrong = r.Fact.Value;
                           RecTmp.NormaSummOfWrong = r.Norma.Value;
                       }

                           DateTime OrderDt = GetDate(r.OrderTime);

                           


                           //Находим все ворктаймы из данного подразделения, к которым относится эта запись
                            List<CEmplWt> WtsOfRec = Wts.Where(a => a.Dep == Dii.Number && a.StartDt < OrderDt && a.StopDt > OrderDt).ToList();
                           //Это для всего подразделения
                            WtsOfRec.Add(
                                new CEmplWt()
                                {
                                    Dep = Dii.Number,
                                    Emp = new CEmpl ()
                                    {
                                    Id=0,
                                    Name = Dii.Name,
                                    },
                                    StartDt = OrderDt.Date,
                                    //StopDt
                                }
                                );

                           foreach (CEmplWt Wt in WtsOfRec)
                           {
                               ReportDayQSRTime Rec = Tmp.FirstOrDefault(a => a.Day.Value == OrderDt.Date && a.Department == Dii.Number && a.EmpId == Wt.Emp.Id);
                               if (Rec == null)
                               {
                                   Rec = new ReportDayQSRTime()
                                   {
                                       EmpId = Wt.Emp.Id,
                                       EmpName = Wt.Emp.Name,
                                       Day = OrderDt.Date,
                                       Department = Dii.Number,
                                       DepName = Dii.Name,
                                        AllTime = 0,
                               OrdersCount = 0,
                               WrongCount = 0,
                               WrongTime = 0,
                               FactSummOfWrong = 0,
                               NormaSummOfWrong = 0
                                   };
                               }
                               Rec.AllTime += RecTmp.AllTime;
                               Rec.OrdersCount += RecTmp.OrdersCount;
                               Rec.WrongCount += RecTmp.WrongCount;
                               Rec.WrongTime += RecTmp.WrongTime;
                               Rec.FactSummOfWrong += RecTmp.FactSummOfWrong;
                               Rec.NormaSummOfWrong += RecTmp.NormaSummOfWrong;
                               if (!Tmp.Contains(Rec))
                               {
                                   Tmp.Add(Rec);
                               }
                               else
                               {
                                   Console.WriteLine("OlRec");
                               }
                            }


                   }
                   

               }
           
           
           
               return Tmp;

       }

        // ToDo можно перейти на C#8.0 (.Net 4.8), там диапазоны реализованы штатно
        public class Range
        {
            public int Min;
            public int Max;
            public Range(int _min, int _max) { Min = _min; Max = _max; }
            public bool InRange(int _value) { return (_value >= Min && _value <= Max); }
        }

        public class Test
        {
            public int trans { get; set; }
            public int maxTime { get; set; }
            public Dictionary<int, int> itemTimes { get; set; }
            //public string itemTimes { get; set; }
        }
        public static Dictionary<int, List<Test>> test = new Dictionary<int, List<Test>>();
        public static Dictionary<int, List<string>> testStrange = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<string>> testbumps = new Dictionary<int, List<string>>();

        internal static List<OrderTimes> GetOrdersOfDepAndDate(DateTime Fdt, DateTime Edt, int Dep, List<int> Items, List<Range> ExcludeTables = null)
        {
            List<OrderTimes> Res = new List<OrderTimes>();
            ReportBaseDataContext db = new ReportBaseDataContext();
            db.CommandTimeout = 1000000;
            Items.Add(0);
            var Recs = db.OrderTimes.Where(a => a.OrderEndTime.Value >= Fdt && a.OrderEndTime.Value <= Edt && a.Dep.Value == Dep ).ToList();
            Recs = Recs.Where(a=>Items.Contains(a.ItemId.Value)).ToList();
            // var Recs = db.OrderTimes.Where(a => a.OrderEndTime.Value >= Fdt && a.OrderEndTime.Value <= Edt && a.Dep.Value == Dep && Items.Contains(a.ItemNumber.Value)).ToList();

            if (ExcludeTables != null)
            {
                // Искл. из списка диапазоны столов доставки
                Recs = Recs.Where(a => ExcludeTables.All(_range => a.TableNum == null || !_range.InRange(a.TableNum.Value))).ToList();
            }

            foreach (DateTime dt in Recs.Select(a=>a.BusinessDate).Distinct())
            {
                foreach (int Trans in Recs.Where(a => a.BusinessDate.Value == dt).Select(b => b.TransactionNumber).Distinct().ToList())
                {
                    try
                    {
                        List<OrderTimes> Recs2 = Recs.Where(a => a.BusinessDate == dt && a.TransactionNumber == Trans).ToList();
                        if (Recs2.Where(a => a.ItemId > 0).Count() == 0) continue;
                       // int MaxTime = Recs2.Select(a => a.OrderLastBumpTime.Value).Max();

                        int MaxItemCookTime = Recs2.Select(a => a.ItemCookTime.Value).Max();
                        int MaxPrepTime = Recs2.Where(a => a.ItemCookTime == MaxItemCookTime && a.OrderLastBumpTime > 0)
                            .Select(a => a.OrderLastBumpTime.Value-a.OrderFirstDisplayedTime.Value).Min(); //Первый бамп самого длинного блюда

                        //if (Dep == 350)
                        //{
                        //    MaxPrepTime = Recs2.Where(a => a.ItemCookTime == MaxItemCookTime && a.OrderLastBumpTime > 0)
                        //    .Select(a => a.OrderLastBumpTime.Value - a.OrderFirstDisplayedTime.Value).Max(); //Для метрополиса последний бамп самого длинного блюда
                        //}

                        if(Dep == 350) //Для метрополиса Первый бамп самого длинного блюда (И отсеивать лишние записи!!!)
                        {
                            var prepIgn = Recs2.Where(a => a.ItemCookTime == MaxItemCookTime && a.OrderLastBumpTime > 0)
                                .Where(_obj => !(_obj.OrderFirstDisplayedTime > 0 && Recs2
                                                .Any(_itm => _itm.ItemId == _obj.ItemId && _itm.ItemCookTime == _obj.ItemCookTime && _itm.OrderLastBumpTime == _obj.OrderFirstDisplayedTime)));
                            if (prepIgn.Count() > 0)
                                MaxPrepTime = prepIgn.Select(a => a.OrderLastBumpTime.Value - a.OrderFirstDisplayedTime.Value).Min();
                        }

                        OrderTimes Ot = new OrderTimes()
                        {
                            BusinessDate = dt,
                            Dep = Dep,
                            ItemCookTime = MaxItemCookTime,
                            OrderEndTime = Recs2.Where(a => a.ItemCookTime == MaxItemCookTime).Select(a => a.OrderEndTime.Value).Max(),
                            OrderLastBumpTime = MaxPrepTime,
                            //OrderFirstDisplayedTime = 
                            TransactionNumber = Trans
                        };
                        Res.Add(Ot);
                }
                    catch(Exception E)
                    {
                        //Console.WriteLine("GetOrdersOfDepAndDate error " + E.Message);
                    }
                }
            }
            //if (Dep==124)//Dep == 350 || Dep == 375)
            //{
            //    ;
            //}
            return Res;   

        }


        internal static List<ReportDayQSRTimeByOrders> GetTimeOfPrepOfOrdersByDayAndSuShef(DateTime Fdt, DateTime Edt)
        {
            Utils.ToLog(String.Format("Запуск GetTimeOfPrepOfOrdersByDayAndSuShef dt1 = {0}, dt2={1} ", Fdt, Edt));
            List<ReportDayQSRTimeByOrders> Tmp = new List<ReportDayQSRTimeByOrders>();
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetSuShefs(Fdt, Edt), Fdt, Edt);
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            List<int> KitchenItems =CubeData.GetKitchenDList(); //GetKitchenItems();
                                                                // TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

            
            //            DepList = new S2010.DepartmentInfo[]   //Подразделения без кубов (пока кубы висят)
            //            {
            //                new S2010.DepartmentInfo() { Number = 104, Name = "Никитская", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 111, Name = "4 Ветра", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 122, Name = "WallStreet", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 130, Name = "Кудринская", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 177, Name = "Неглинная", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 180, Name = "КИНКИ", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 190, Name = "Депо", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 195, Name = "Вишневые сады", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 205, Name = "Аврора", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 212, Name = "Шереметьево1", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 222, Name = "ШереметьевоУ", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 224, Name = "ШереметьевоБ", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 226, Name = "ШерB Узбечка", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 235, Name = "Павлово", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 237, Name = "Горки-2", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 255, Name = "Чипсайд", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 260, Name = "Якиманка", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 264, Name = "Мэрия", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 270, Name = "Весна", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 280, Name = "Мосс", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 285, Name = "Усачева", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 290, Name = "Хорошо", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 295, Name = "Комсомольский", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 300, Name = "Дом-2", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 310, Name = "Внуково", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 311, Name = "Новая Рига", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 350, Name = "Метрополис", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 370, Name = "Кутузовский", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 371, Name = "Новая площадь", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 375, Name = "Тверская", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 380, Name = "ГУМ", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 390, Name = "БелаяПлощадь", Enabled = true },
            //new S2010.DepartmentInfo() { Number = 395, Name = "Покровка", Enabled = true },
            //            };

            foreach (S2010.DepartmentInfo Dii in DepList.Where(a=>a.Enabled))
            {
                // //  if (!Dii.Enabled) continue;
                //if (Dii.Number != 350 ) continue;
                if (Dii.Number == 197 && Fdt == new DateTime(2021,10,25))
                {
                    ;
                }
                Console.WriteLine(Dii.Name);
                Utils.ToLog(String.Format("GetTimeOfPrepByDayAndSuShef Начало расчета подразделение  {0} {1}", Dii.Number, Dii.Name));

                List<OrderTimes> Res = GetOrdersOfDepAndDate(Fdt, Edt, Dii.Number, KitchenItems);


                Utils.ToLog($"Подразделение  {Dii.Number} {Dii.Name} получил {Res.Count} записей");
                foreach (OrderTimes  r in Res)
                {
                    //  if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                    //if (!KitchenItems.Contains(Convert.ToInt32(r.BarCode))) continue;

                    ReportDayQSRTime RecTmp = new ReportDayQSRTime
                    {
                        OrdersCount = 1,
                        AllTime = r.OrderLastBumpTime - r.OrderFirstDisplayedTime.GetValueOrDefault(),
                        WrongCount = 0,
                        WrongTime = 0,
                        FactSummOfWrong = 0,
                        NormaSummOfWrong = 0
                    };
                    //if (Dii.Number == 350) {
                    //    RecTmp.AllTime = r.OrderLastBumpTime;
                    //}

                    if (r.OrderLastBumpTime> r.ItemCookTime)
                    {
                        RecTmp.WrongCount = 1;
                        RecTmp.WrongTime = r.OrderLastBumpTime.Value - r.ItemCookTime.Value;
                        RecTmp.FactSummOfWrong = r.OrderLastBumpTime.Value ;
                        RecTmp.NormaSummOfWrong = r.ItemCookTime.Value;
                    }

                    DateTime OrderDt = r.OrderEndTime.Value;
                    
                    //Находим все ворктаймы из данного подразделения, к которым относится эта запись
                    List<CEmplWt> WtsOfRec = Wts.Where(a => a.Dep == Dii.Number && a.StartDt < OrderDt && a.StopDt > OrderDt).ToList();
                    //Это для всего подразделения
                    WtsOfRec.Add(
                        new CEmplWt()
                        {
                            Dep = Dii.Number,
                            Emp = new CEmpl()
                            {
                                Id = 0,
                                Name = Dii.Name,
                            },
                            StartDt = OrderDt.Date,
                                    //StopDt
                                }
                        );

                    foreach (CEmplWt Wt in WtsOfRec)
                    {
                        ReportDayQSRTimeByOrders Rec = Tmp.FirstOrDefault(a => a.Day.Value == OrderDt.Date && a.Department == Dii.Number && a.EmpId == Wt.Emp.Id);
                        if (Rec == null)
                        {
                            Rec = new ReportDayQSRTimeByOrders()
                            {
                                EmpId = Wt.Emp.Id,
                                EmpName = Wt.Emp.Name,
                                Day = OrderDt.Date,
                                Department = Dii.Number,
                                DepName = Dii.Name,
                                AllTime = 0,
                                OrdersCount = 0,
                                WrongCount = 0,
                                WrongTime = 0,
                                FactSummOfWrong = 0,
                                NormaSummOfWrong = 0
                            };
                        }
                        Rec.AllTime += RecTmp.AllTime;
                        Rec.OrdersCount += RecTmp.OrdersCount;
                        Rec.WrongCount += RecTmp.WrongCount;
                        Rec.WrongTime += RecTmp.WrongTime;
                        Rec.FactSummOfWrong += RecTmp.FactSummOfWrong;
                        Rec.NormaSummOfWrong += RecTmp.NormaSummOfWrong;
                        if (!Tmp.Contains(Rec))
                        {
                            Tmp.Add(Rec);
                        }

                        //if(Rec.Department == 197 && (new List<DateTime>() { new DateTime(2021, 10, 9), new DateTime(2021, 10, 10), new DateTime(2021, 10, 11) }).Contains((DateTime)Rec.Day))
                        //{
                        //    Rec.WrongCount /= 10;
                        //    Rec.WrongTime /= 10;
                        //    Rec.FactSummOfWrong /= 10;
                        //    Rec.NormaSummOfWrong /= 10;
                        //}


                    }


                }


            }



            return Tmp;

        }

        internal static void GetAvgTimeOfPrepByDeps(DateTime Fdt, DateTime Edt)
       {
          
           List<ReportDayQSRTime> Tmp = new List<ReportDayQSRTime>();
          // List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetSuShefs(Fdt, Edt), Fdt, Edt);
           S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
           S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
           List<int> HotItems = GetHotItems();
           List<int> ColdItems = GetColdItems();
           TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();


           Application app = new Microsoft.Office.Interop.Excel.Application();
           Workbook Wb = app.Workbooks.Add(true);
           Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
           app.Visible = true;
           //Ws.Cells[2, 1] = "Критерии";

           int row = 2;
           foreach (S2010.DepartmentInfo Dii in DepList)
           {
               Ws.Cells[row, 2] = Dii.Name;
               if (!Dii.Enabled) continue;
             
               Console.WriteLine(Dii.Name);
           
               TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[1];

               int? kol = 0;
               PrepSrv.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
               PrepSrv.ShopsGoodTime(Dii.Number, Fdt, Edt.AddDays(-1), out kol, out res);

               int AllTimeHot = 0;
               int AllTimeHotCount = 0;
               int AllTimeCold = 0;
               int AllTimeColdCount = 0;

               foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res)
               {
               
                   if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                   if (HotItems.Contains(Convert.ToInt32(r.BarCode)))
                   {
                       AllTimeHot += r.Fact.Value;
                       AllTimeHotCount++;
                   }
                   else if (ColdItems.Contains(Convert.ToInt32(r.BarCode)))
                   {
                       AllTimeCold += r.Fact.Value;
                       AllTimeColdCount++;
                   }
                }
               Double AvgTimeHot = 0;
               Double AvgTimeCold = 0;
               if (AllTimeHotCount > 0)
               {
                   AvgTimeHot = AllTimeHot / AllTimeHotCount;
               }
               if (AllTimeColdCount > 0)
               {
                   AvgTimeCold = AllTimeCold / AllTimeColdCount;
               }
               Ws.Cells[row, 3] = AvgTimeCold;
               Ws.Cells[row, 4] = AvgTimeHot;
               row++;
           }



           Wb.Save();

       }



       internal static DateTime GetDate(string Date)
       {
           return new DateTime(int.Parse(Date.Substring(6, 2))+2000, int.Parse(Date.Substring(3, 2)), int.Parse(Date.Substring(0, 2)), int.Parse(Date.Substring(9, 2)), int.Parse(Date.Substring(12, 2)), int.Parse(Date.Substring(15, 2)));
       }


        internal static List<PrepTime> GetTimeOfPrepOrder(DateTime Fdt, DateTime Edt, bool ExcludeDeliveryTables = false)
        {
            List<PrepTime> Tmp = new List<PrepTime>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();


            // ToDo - это хард, диапазоны столов доставки надо брать из базы
            List<Range> excludeTables = null;
            if (ExcludeDeliveryTables)
                excludeTables = new List<Range>() { new Range(146, 254), new Range(900, 929) };


            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();



            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                //if (Dii.Number != 350 && Dii.Number != 395 && Dii.Number != 375 && Dii.Number != 285 && Dii.Number != 295 && Dii.Number != 371)
                //    continue;

                List<OrderTimes> Res = TimeOfPreparation.GetOrdersOfDepAndDate(Fdt, Edt, Dii.Number, KitchenItems, excludeTables);
                PrepTime Pt = new PrepTime()
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name

                };


                foreach (OrderTimes r in Res)
                {
                    Pt.AllCount++;
                    
                    Pt.FactSumm += r.OrderLastBumpTime.Value;
                    Pt.NormaSumm += r.ItemCookTime.Value;
                    

                    if (r.OrderLastBumpTime > r.ItemCookTime)
                    {
                        Pt.WrongSecond += r.OrderLastBumpTime.Value - r.ItemCookTime.Value;
                        Pt.WrongCount++;
                    }

                    //if (Dii.Number == 197 && (new List<DateTime>() { new DateTime(2021, 10, 9), new DateTime(2021, 10, 10), new DateTime(2021, 10, 11) }).Contains(Fdt))
                    //    Pt.WrongCount /= 10;
                }

             //   decimal AllDCount = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                decimal WrongPercent = 0;
                if (Pt.AllCount != 0)
                {
                    WrongPercent = (decimal)Pt.WrongCount / (decimal)Pt.AllCount;
                };

                Tmp.Add(Pt);
            }
            return Tmp;
        }


       internal static List<PrepTime> GetTimeOfPrep(DateTime Fdt, DateTime Edt)
       {
            //List<int> KitchenItems = GetKitchenItems();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();
            List<PrepTime> Tmp = new List<PrepTime>();

           S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
           S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

           TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

           foreach (S2010.DepartmentInfo Dii in DepList)
           {
               if (!Dii.Enabled) continue;
               //if (Dii.Number != 197) continue;

               Console.WriteLine(Dii.Name);
               
               List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow> res2 = new List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow>();

               int? kol = 0;
               TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
               PrepSrv.ShopsGoodTime(Dii.Number, Fdt, Edt.AddDays(-1), out kol, out res);
               /*
               for (DateTime dt = Fdt; dt < Edt; dt = dt.AddDays(1))
               {
                   Console.WriteLine(Dii.Name + " " + dt.ToString("dd/MM/yyyy"));
                   TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                   int? kol = 0;
                   PrepSrv.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                   PrepSrv.ShopsGoodTime(Dii.Number, dt, dt.AddDays(1), out kol, out res);

                   res2.AddRange(res.ToList());
               }
                * */
               PrepTime Pt = new PrepTime ()
               {
                   Dep = Dii.Number,
                   DepName = Dii.Name
               
               };
               
               //foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res2)


               foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res)
               {
                   if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                   if (!KitchenItems.Contains(Convert.ToInt32( r.BarCode))) continue;
                   Pt.AllCount++;
                   if (r.Fact > r.Norma)
                   {
                       Pt.WrongCount++;
                       Pt.FactSumm+= r.Fact.Value;
                       Pt.NormaSumm+= r.Norma.Value;
                       Pt.WrongSecond += r.Fact.Value - r.Norma.Value;
                   }

               }
               Tmp.Add(Pt);
               
           }
           
           return Tmp;

       }


        internal static void GetAvgTimeOfPrepByCat(DateTime Fdt, DateTime Edt, int Cat)
        {
            List<PrepTime> res = new List<PrepTime>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            foreach (S2010.DepartmentInfo Dii in DepList.Where(a=>a.Place.Trim().ToLower()=="город"))
            {
                res.AddRange(GetTimeOfPrepByPointAndCat(Fdt, Edt, Dii.Number, Cat));
            }

            double ress = res.Sum(a => a.FactSumm)/ res.Sum(a => a.AllCount);

            
            Console.WriteLine(ress);
            Console.Read()
;        }


       internal static List<PrepTime> GetTimeOfPrepByPointAndCat(DateTime Fdt, DateTime Edt, int Point, int Cat)
       {
            //List<int> KitchenItems = GetItemsByCat(Cat);

            List<int> KitchenItems = CubeData.GetDishOfCat(Cat).Select(a=>Convert.ToInt32(a)).ToList();

           List<PrepTime> Tmp = new List<PrepTime>();

           S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
           //S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

           TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

           //foreach (S2010.DepartmentInfo Dii in DepList)
           {
           //    if (!Dii.Enabled) continue;
//               if (Dii.Number != 260) continue;

  //             Console.WriteLine(Dii.Name);
               TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];

               int? kol = 0;
               PrepSrv.ShopsGoodTime(Point, Fdt, Edt.AddDays(-1), out kol, out res);
               
               PrepTime Pt = new PrepTime()
               {
                   Dep = Point,
                   DepName = ""

               };
               
               foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res)
               {
                   if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                   if (!KitchenItems.Contains(Convert.ToInt32(r.BarCode))) continue;
                   Pt.AllCount++;
                  // if (r.Fact > r.Norma)
                   {
                       Pt.WrongCount++;
                       Pt.FactSumm += r.Fact.Value;
                       Pt.NormaSumm += r.Norma.Value;
                       Pt.WrongSecond += r.Fact.Value - r.Norma.Value;
                   }

               }
               Tmp.Add(Pt);
               //Console.Write(Pt.FactSumm / Pt.AllCount);
           }

           return Tmp;

       }

       internal static List<int> GetKitchenItems()
        {
            //ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=quasiadm;Password=Fil123fil123");
            List<int> KitchenGroups = new List<int>() {8,9,10,12,11,13,14,15,16,23,25,26,27,28,29,30,31,33,34 };
           List<int> Tmp = new List<int>();



           IQueryable<int> KitchenGroupsItems = from o in RepBase.AlohaMenuITMs  
                                                where KitchenGroups.Contains(o.Category.Value) 
                                                select o.ID.Value;

           return KitchenGroupsItems.Distinct().ToList();
       }
       internal static List<int> GetHotItems()
        {
            //ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=quasiadm;Password=Fil123fil123");
            List<int> KitchenGroups = new List<int>() { 9, 12, 11, 13, 14, 15, 16, 23, 25, 26, 27, 28, 29, 30, 31, 33, 34 };
           List<int> Tmp = new List<int>();



           IQueryable<int> KitchenGroupsItems = from o in RepBase.AlohaMenuITMs
                                                where KitchenGroups.Contains(o.Category.Value)
                                                select o.ID.Value;

           return KitchenGroupsItems.Distinct().ToList();
       }

       internal static List<int> GetColdItems()
        {
            //ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=quasiadm;Password=Fil123fil123");
            List<int> KitchenGroups = new List<int>() {10};
           List<int> Tmp = new List<int>();



           IQueryable<int> KitchenGroupsItems = from o in RepBase.AlohaMenuITMs
                                                where KitchenGroups.Contains(o.Category.Value)
                                                select o.ID.Value;

           return KitchenGroupsItems.Distinct().ToList();
       }



       internal static List<int> GetItemsByCat(int Cat)
        {
            //ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=quasiadm;Password=Fil123fil123");
            List<int> KitchenGroups = new List<int>() { Cat };
           List<int> Tmp = new List<int>();

           IQueryable<int> KitchenGroupsItems = from o in RepBase.AlohaMenuITMs
                                                where KitchenGroups.Contains(o.Category.Value)
                                                select o.ID.Value;

           return KitchenGroupsItems.Distinct().ToList();
       }
    }
   
}
