using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ReportMonthResultGenerator.QSR
{
    public static class QSRTiming
    {
        /*
        public static ServTimeXml ReadQSRXML(string Path)
        {

            ServTimeXml res = new ServTimeXml();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServTimeXml));
            using (StreamReader reader = new StreamReader(Path))
            {
                res = (ServTimeXml)xmlSerializer.Deserialize(reader);
            }
            return res;
        }
        */


        public static QSRPercentRes GetAvgQSRPercentAllDir(string Path)
        {
            DirectoryInfo di = new DirectoryInfo(Path);
            QSRPercentRes Res = new QSRPercentRes();
            foreach (FileInfo fi in di.GetFiles())
            {
                QSRPercentRes r1 = GetAvgQSRPercent(fi.FullName);
                Res.All += r1.All;
                Res.Wrong += r1.Wrong;
            }
            Console.WriteLine(Res.All + " " + Res.Wrong + " " + Res.Percent + " ");
            Console.Read();
            return Res;
        }

        public static void OtTest()
        {
            SrvOrderTimes.SrvOrderTimes2SoapClient srv = new SrvOrderTimes.SrvOrderTimes2SoapClient();
            SrvOrderTimes.OrderTimes ot = new SrvOrderTimes.OrderTimes()
            {
                BusinessDate = new DateTime(2018, 1, 1),
                Dep = 200,
                //...
            };
            var result = srv.AddOrderTimes(ot);
            Console.WriteLine($" result = {result.Result}; Error = {result.ErrMsg} ");
         }

        public static void InsertOrderTimeRecordsAllDeps(string DPath, int daysDeep, DateTime EndDate)
        {
            DirectoryInfo di = new DirectoryInfo(DPath);
            foreach (DirectoryInfo Di in di.GetDirectories())
            {
                //if (QSRXMLDepExist(Convert.ToInt32(Di.Name))) continue;
                //if (Convert.ToInt32(Di.Name)!=450) continue;
                //if (Convert.ToInt32(Di.Name) == 104) continue;
                //if (Convert.ToInt32(Di.Name) < 270) continue;

                var dn=0;
                if (Int32.TryParse(Di.Name,out dn))
                {
                    InsertOrderTimeRecordsAllDir(Di.FullName, dn, daysDeep, EndDate);
                }
            }
        }
        
        public static void InsertOrderTimeRecordsAllDir(string DPath, int Dep , int daysDeep,DateTime EndDate)
        {
            DirectoryInfo di = new DirectoryInfo(DPath);

            if (!di.Exists) return;

            Utils.ToLog($"InsertOrderTimeRecordsAllDir {DPath}, daysDeep: {daysDeep}", true);

            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                string fName = "";
                if (fi.Name.Substring(0, 3) == "SOS")
                {
                    fName = fi.Name.Substring(3);
                }
                else
                {
                    fName = fi.Name.Substring(2);
                }


                if (fName.Length > 12) { continue; }
                try
                {
                    int Y = Convert.ToInt32(fName.Substring(0, 4));
                    int M = Convert.ToInt32(fName.Substring(4, 2));
                    int D = Convert.ToInt32(fName.Substring(6, 2));
                    DateTime bd = new DateTime(Y, M, D).AddDays(-1);
                    if (fi.LastWriteTime.Hour < 10)
                    {
                        bd = bd.AddDays(-1);
                    }
                    if (bd < EndDate.AddDays(-daysDeep-1 )) continue;
                    if (bd > EndDate) continue;

                    List<string> Paths = di.GetFiles().Where(a => a.Name.Contains(Path.GetFileNameWithoutExtension(fi.Name))).Select(a => di.FullName + @"\" + a.Name).ToList();

                    InsertQSRXMLInTable(Paths, Dep, bd);

                    Utils.ToLog("InsertQSRXMLInTable " + fi.FullName, true);
                }
                catch(Exception e )
                {
                    Utils.ToLog($"Skip {fi.FullName} Err {e.Message}", true);
                }
            }

        }

        public static QSRPercentRes GetAvgQSRPercent(string Path)
        {
            ServTimeXml ST = ReadQSRXML(Path);

            Dictionary<int, CServiceTiming> Sts = new Dictionary<int, CServiceTiming>();
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();
            foreach (CServiceTiming St in ST.ServiceTiming)
            {
                if (!KitchenItems.Contains(St.ItemId)) continue;
                CServiceTiming StInto = new CServiceTiming();
                if (Sts.TryGetValue(St.TransactionNumber, out StInto))
                {
                    if (StInto.ItemCookTime < St.ItemCookTime)
                    {
                        Sts.Remove(St.TransactionNumber);
                        Sts.Add(St.TransactionNumber, St);
                    }
                }
                else
                {
                    Sts.Add(St.TransactionNumber, St);
                }
            }
            int All = Sts.Values.Count();
            int Wrong = Sts.Values.Where(a => a.ItemCookTime < a.OrderLastBumpTime - a.OrderFirstDisplayedTime).Count();

            Console.WriteLine(All + " " + Wrong);
            //Console.Read();
            QSRPercentRes res = new QSRPercentRes()
            {
                All = All,
                Wrong = Wrong
            };

            return res;
        }



        public static ServTimeXml ReadQSRXML(string Path)
        {
            ServTimeXml Res = new ServTimeXml();
            Res.ServiceTiming = new List<CServiceTiming>();
            string XmlRes = "";

            using (StreamReader reader = new StreamReader(Path))
            {
                XmlRes = reader.ReadToEnd();
            }
            XmlRes = XmlRes.Replace("&", " ");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XmlRes);



            foreach (XmlNode xn in doc.FirstChild.ChildNodes)
            {
                try
                {
                    CServiceTiming Tim = new CServiceTiming();

                    Tim = new CServiceTiming()
                    {
                        ItemCookTime = Convert.ToInt32(xn.SelectSingleNode("ItemCookTime").InnerText),
                        TransactionNumber = Convert.ToInt32(xn.SelectSingleNode("TransactionNumber").InnerText),
                        ItemNumber = Convert.ToInt32(xn.SelectSingleNode("ItemNumber").InnerText),
                        ItemId = Convert.ToInt32(xn.SelectSingleNode("ItemId").InnerText),
                        OrderFirstDisplayedTime = Convert.ToInt32(xn.SelectSingleNode("OrderFirstDisplayedTime").InnerText),
                        OrderLastBumpTime = Convert.ToInt32(xn.SelectSingleNode("OrderLastBumpTime").InnerText),
                        VirtualDisplayId = Convert.ToInt32(xn.SelectSingleNode("VirtualDisplayId").InnerText),
                        ServerId = Convert.ToInt32(xn.SelectSingleNode("ServerId").InnerText),
                    };
                    XmlNode Xdate = xn.SelectSingleNode("Order_Start_Time");
                    int Y = Convert.ToInt32(Xdate.SelectSingleNode("Year").InnerText);
                    int M = Convert.ToInt32(Xdate.SelectSingleNode("Month").InnerText);
                    int D = Convert.ToInt32(Xdate.SelectSingleNode("Day").InnerText);
                    int H = Convert.ToInt32(Xdate.SelectSingleNode("Hour").InnerText);
                    int m = Convert.ToInt32(Xdate.SelectSingleNode("Minute").InnerText);
                    int S = Convert.ToInt32(Xdate.SelectSingleNode("Second").InnerText);
                    Tim.Order_Start_Time = new DateTime(Y, M, D, H, m, S);


                    Res.ServiceTiming.Add(Tim);
                }
                catch
                {

                }
            }
            return Res;
        }


        public static bool QSRXMLDepExist(int dep)
        {
            ReportBaseDataContext db = new ReportBaseDataContext();
            return db.OrderTimes.Where(a => a.Dep == dep).Count() >0;
        }


        public static bool QSRXMLRecordNotExists(OrderTimes rec)
        {
            ReportBaseDataContext db = new ReportBaseDataContext();
            return db.OrderTimes.Where(a => a.Dep == rec.Dep && a.OrderStartTime == rec.OrderStartTime && a.VirtualDisplayId == rec.VirtualDisplayId && a.ItemNumber == rec.ItemNumber && a.TransactionNumber == rec.TransactionNumber).Count() == 0;
        }

        public static void InsertQSRXMLInTable(List<string> Paths, int Dep, DateTime DateOfBusiness)
        {
            List<OrderTimes> Tmp = new List<OrderTimes>();
            ServTimeXml Res = new ServTimeXml();
            Res.ServiceTiming = new List<CServiceTiming>();
            string XmlRes = "";
            foreach (string s in Paths)
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    XmlRes = reader.ReadToEnd() + Environment.NewLine;
                }

                XmlRes = XmlRes.Replace("&", " ");
                XmlRes = XmlRes.Replace("%", " ");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(XmlRes);


                ReportBaseDataContext db = new ReportBaseDataContext();
                db.CommandTimeout = 600000;
                foreach (XmlNode xn in doc.FirstChild.ChildNodes)
                {

                    try
                    {
                        // CServiceTiming Tim = new CServiceTiming();

                        OrderTimes Tim = new OrderTimes()
                        {
                            ItemCookTime = Convert.ToInt32(xn.SelectSingleNode("ItemCookTime").InnerText),
                            TransactionNumber = Convert.ToInt32(xn.SelectSingleNode("TransactionNumber").InnerText),
                            ItemNumber = Convert.ToInt32(xn.SelectSingleNode("ItemNumber").InnerText),
                            ItemId = Convert.ToInt32(xn.SelectSingleNode("ItemId").InnerText),
                            OrderFirstDisplayedTime = Convert.ToInt32(xn.SelectSingleNode("OrderFirstDisplayedTime").InnerText),
                            OrderLastBumpTime = Convert.ToInt32(xn.SelectSingleNode("OrderLastBumpTime").InnerText),
                            
                            Dep = Dep,
                            BusinessDate = DateOfBusiness,

                        };

                        try
                        {
                            Tim.TableNum = Convert.ToInt32(xn.SelectSingleNode("TableNumber").InnerText);

                                }
                        catch
                        { }
                        try
                        {
                            Tim.VirtualDisplayId = Convert.ToInt32(xn.SelectSingleNode("VirtualDisplayId").InnerText);
                        }
                        catch
                        { }
                        try { Tim.ServerId = Convert.ToInt32(xn.SelectSingleNode("ServerId").InnerText); }
                        catch
                        { }

                        XmlNode Xdate = xn.SelectSingleNode("Order_Start_Time");
                        if (Xdate == null)
                        {
                            Xdate = xn.SelectSingleNode("OrderStartTime");
                        }
                        int Y = Convert.ToInt32(Xdate.SelectSingleNode("Year").InnerText);
                        int M = Convert.ToInt32(Xdate.SelectSingleNode("Month").InnerText);
                        int D = Convert.ToInt32(Xdate.SelectSingleNode("Day").InnerText);
                        int H = Convert.ToInt32(Xdate.SelectSingleNode("Hour").InnerText);
                        int m = Convert.ToInt32(Xdate.SelectSingleNode("Minute").InnerText);
                        int S = Convert.ToInt32(Xdate.SelectSingleNode("Second").InnerText);
                        Tim.OrderStartTime = new DateTime(Y, M, D, H, m, S);
                        Tim.OrderEndTime = Tim.OrderStartTime.Value.AddSeconds(Tim.OrderLastBumpTime.Value);

                        Tmp.Add(Tim);
                        /*
                        if (QSRXMLRecordNotExists(Tim))
                        {
                            db.OrderTimes.InsertOnSubmit(Tim);
                        }
                        */


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                DateTime MinDt = Tmp.Select(a => a.OrderStartTime).Min().Value;
                DateTime MaxDt = Tmp.Select(a => a.OrderStartTime).Max().Value;
                List<OrderTimes> InBase = db.OrderTimes.Where(a => a.Dep == Dep && a.OrderStartTime >= MinDt && a.OrderStartTime <= MaxDt).ToList();

                //Tmp.RemoveAll(a=>a)

                List<OrderTimes> Tmp2 = new List<OrderTimes>();
                foreach (OrderTimes rec in Tmp)
                {
                    if (InBase.Any(a => a.Dep == rec.Dep && a.OrderStartTime == rec.OrderStartTime && a.VirtualDisplayId == rec.VirtualDisplayId && a.ItemNumber == rec.ItemNumber && a.TransactionNumber == rec.TransactionNumber))
                    {

                        Tmp2.Add(rec);
                    }
                }

                Tmp.RemoveAll(a => Tmp2.Contains(a));
                if (Tmp.Count() > 0)
                {
                    Utils.ToLog($"Inserted {Tmp.Count() } records", true);
                    db.OrderTimes.InsertAllOnSubmit(Tmp);
                    db.SubmitChanges();
                }
                else
                {
                    Console.WriteLine("No Unik");
                }

                
            }
            //return Res;
        }
       
    }
    public class QSRPercentRes
    {
        public int All { set; get; }
        public int Wrong { set; get; }
        public double Percent
        {
            get
            {
                if (All == 0) return 0;
                return ((double)Wrong) / ((double)All);
            }
        }
    }

}


