using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Xml;
using System.IO;


namespace ReportMonthResultGenerator
{
    class KissTheCook
    {
        public static void TestReport(DateTime Fdt, DateTime Edt)
        {
            //List<int> KitchenItems =  GetKitchenItems();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();

            List<PrepTime> Tmp = new List<PrepTime>();

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            //TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();
            //PrepSrv.InnerChannel.OperationTimeout = new TimeSpan (0,30,0);



        //    List<DishCount> Dk = CubeData.GetKitchenDishesCount(Fdt);


            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            int row = 1;

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
//                if (!Dii.Enabled) continue;
                //if (Dii.Number != 370) continue;
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim().ToLower() != "город") continue;

                Console.WriteLine(Dii.Name);

                List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow> res2 = new List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow>();

                PrepTime Pt = new PrepTime()
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name

                };

                List<OrderTimes> Res = TimeOfPreparation.GetOrdersOfDepAndDate(Fdt, Edt, Dii.Number, KitchenItems);

                foreach (OrderTimes r in Res)
                {
                    Pt.AllCount++;
                    if (r.OrderLastBumpTime > r.ItemCookTime)
                    {
                        Pt.WrongCount++;
                    }

                }
                
                Ws.Cells[row, 1] = Dii.Name; //Ресторан
                try
                {
                    Ws.Cells[row, 2] = Pt.AllCount; //Кол-во блюд
                    Ws.Cells[row, 3] = Pt.WrongCount; //Просроченные от блюд
                    //Ws.Cells[row, 5] = Dk.Where(a=>a.Dep==Dii.Number).Sum(a=>a.Count); //Все блюда
                }
                catch
                { }
                row++;


                //Tmp.Add(Pt);

            }

            //return Tmp;

        


    }



        public static void TestReportByDay(DateTime Fdt)
        {


            //List<int> KitchenItems =  GetKitchenItems();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();

            List<PrepTime> Tmp = new List<PrepTime>();

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();
            PrepSrv.InnerChannel.OperationTimeout = new TimeSpan(0, 30, 0);
            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Fdt,false,Fdt.Day);


            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            int row = 1;

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                //                if (!Dii.Enabled) continue;
             //   if (Dii.Number != 130) continue;
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim().ToLower() != "город") continue;

                Console.WriteLine(Dii.Name);

                List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow> res2 = new List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow>();

                int? kol = 0;
                TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                PrepSrv.ShopsGoodTime(Dii.Number, Fdt.Date, Fdt.Date, out kol, out res);
                PrepTime Pt = new PrepTime()
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name

                };

                //foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res2)
                foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res)
                {
                    //  if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                    if (!KitchenItems.Contains(Convert.ToInt32(r.BarCode))) continue;
                    Pt.AllCount++;
                    if (r.Fact > r.Norma)
                    {
                        Pt.WrongCount++;
                        Pt.FactSumm += r.Fact.Value;
                        Pt.NormaSumm += r.Norma.Value;
                        Pt.WrongSecond += r.Fact.Value - r.Norma.Value;
                    }

                }

                Ws.Cells[row, 1] = Dii.Name; //Ресторан
                try
                {
                    Ws.Cells[row, 2] = Pt.AllCount; //Кол-во блюд
                    Ws.Cells[row, 3] = Pt.WrongCount; //Просроченные от блюд
                    Ws.Cells[row, 5] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count); //Все блюда
                }
                catch
                { }
                row++;


                //Tmp.Add(Pt);

            }

            //return Tmp;




        }



        public static void TestReportByDayAllDishes(DateTime Fdt)
        {


            //List<int> KitchenItems =  GetKitchenItems();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();

            List<PrepTime> Tmp = new List<PrepTime>();

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            TimeOfPrep.Ges3ServicesObjClient PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();
            PrepSrv.InnerChannel.OperationTimeout = new TimeSpan(0, 30, 0);
            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Fdt, false, Fdt.Day);


            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            int row = 1;

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                //                if (!Dii.Enabled) continue;
                   if (Dii.Number != 295) continue;
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim().ToLower() != "город") continue;

                Console.WriteLine(Dii.Name);

                List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow> res2 = new List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow>();

                int? kol = 0;
                TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                PrepSrv.ShopsGoodTime(Dii.Number, Fdt.Date, Fdt.Date, out kol, out res);
                PrepTime Pt = new PrepTime()
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name

                };

                //foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res2)
                foreach (TimeOfPrep.ShopsGoodTime_T_goodtimeRow r in res)
                {
                    //  if ((r.Fact.Value - r.Norma.Value) > 1200) continue;
                    if (!KitchenItems.Contains(Convert.ToInt32(r.BarCode))) continue;
                    Pt.AllCount++;
                    if (r.Fact > r.Norma)
                    {
                        Pt.WrongCount++;
                        Pt.FactSumm += r.Fact.Value;
                        Pt.NormaSumm += r.Norma.Value;
                        Pt.WrongSecond += r.Fact.Value - r.Norma.Value;
                    }
                    int Cell = 1;
                    Ws.Cells[row, Cell++] = r.OrderTime;
                    Ws.Cells[row, Cell++] = r.BarCode;
                    Ws.Cells[row, Cell++] = r.Fact;
                    Ws.Cells[row, Cell++] = r.Norma;
                    Ws.Cells[row, Cell++] = r.Norma< r.Fact ? 1:0;
                    row++;

                }
                /*
                Ws.Cells[row, 1] = Dii.Name; //Ресторан
                try
                {
                    Ws.Cells[row, 2] = Pt.AllCount; //Кол-во блюд
                    Ws.Cells[row, 3] = Pt.WrongCount; //Просроченные от блюд
                    Ws.Cells[row, 5] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count); //Все блюда
                }
                catch
                { }
                row++;
                */

                //Tmp.Add(Pt);

            }

            //return Tmp;




        }



        public static List<QSRDishInfo> GetAvgTimeDataRussia(DateTime SrartDate, DateTime EndDate)
        {
            string DirPath = @"D:\NYRep\lesn\";
            List<QSRDishInfo> Tmp = new List<QSRDishInfo>();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();

            foreach (FileInfo fi in new DirectoryInfo(DirPath).GetFiles("*.xml").Where(a => a.LastWriteTime < EndDate.AddDays(2) && a.LastWriteTime > SrartDate.AddDays(-2)))
            {
                try
                {
                    StreamReader sr = new StreamReader(fi.FullName);

                    string sx = sr.ReadToEnd();
                    sx = sx.Replace("&", " ");


                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(sx);
                    foreach (XmlNode xn in doc.FirstChild.ChildNodes)
                    {
                        try
                        {
                            if (xn.SelectSingleNode("DestinationName").InnerText == "DONT_MAKE")
                            {
                                continue;
                            }
                            int StType = Convert.ToInt32(xn.SelectSingleNode("StationType").InnerText);
                            if (StType != 1)
                            {
                                continue;
                            }
                            XmlNode XnTimeStamp = xn.SelectSingleNode("TimeStamp");
                            int y = Convert.ToInt32(XnTimeStamp.SelectSingleNode("Year").InnerText);
                            int m = Convert.ToInt32(XnTimeStamp.SelectSingleNode("Month").InnerText);
                            int d = Convert.ToInt32(XnTimeStamp.SelectSingleNode("Day").InnerText);
                            DateTime dt = new DateTime(y, m, d);
                            if ((dt < SrartDate) || (dt > EndDate))
                            {
                                continue;
                            }

                            int Bc = Convert.ToInt32(xn.SelectSingleNode("ItemId").InnerText);

                            if (!KitchenItems.Contains(Bc))
                            {
                                continue;
                            }

                            QSRDishInfo Qd = new QSRDishInfo();
                            if (Tmp.Where(a => a.BarCode == Bc).Count() > 0)
                            {
                                Qd = Tmp.Where(a => a.BarCode == Bc).First();
                            }
                            else
                            {

                                Qd.BarCode = Bc;
                                Qd.Name = xn.SelectSingleNode("ItemDescription").InnerText;
                                Qd.Norm = Convert.ToInt32(xn.SelectSingleNode("ItemCookTime").InnerText);
                                Qd.Cat = Convert.ToInt32(xn.SelectSingleNode("ItemCategory").InnerText);
                                if ((Qd.Cat != 999) && (Qd.BarCode != 0))
                                {
                                    Tmp.Add(Qd);
                                }

                            }
                            Qd.TimeOfPrepCount++;
                            int StartTime = Math.Max(Convert.ToInt32(xn.SelectSingleNode("OrderFirstDisplayedTime").InnerText), 0);
                            int EndTime = Convert.ToInt32(xn.SelectSingleNode("OrderLastBumpTime").InnerText);
                            if (EndTime == 0) continue;
                            int diff = EndTime - StartTime;
                            Qd.TimeOfPrepSumm += (EndTime - StartTime);

                            if (diff > Qd.Norm)
                            {
                                Qd.WrongCount++;
                                Qd.WrongSumm += (diff - Qd.Norm);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                { }
            }
            Console.WriteLine(String.Format("All {0} Wrong {1}", Tmp.Sum(a=>a.TimeOfPrepCount),Tmp.Sum(a=>a.WrongCount)));
            Console.ReadKey();
            return Tmp;
        }

    }
}
