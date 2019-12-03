using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Office.Interop.Excel;

namespace ReportMonthResultGenerator
{
    class NYQSR
    {
        public static void CreateReport()
        {
            List<QSRDishInfo> Tmp = new List<QSRDishInfo>();
            String P = @"D:\NYRep\Qsr";
            foreach (FileInfo fi in new DirectoryInfo(P).GetFiles())
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
                        int Bc = Convert.ToInt32 (xn.SelectSingleNode("ItemId").InnerText);
                        QSRDishInfo Qd = new QSRDishInfo();
                        if (Tmp.Where(a => a.BarCode == Bc).Count() > 0)
                        {
                            Qd = Tmp.Where(a => a.BarCode == Bc).First();
                        }
                        else
                        {
                            Tmp.Add(Qd);
                            Qd.BarCode = Bc;
                            Qd.Name = xn.SelectSingleNode("ItemDescription").InnerText;
                            Qd.Norm =Convert.ToInt32(xn.SelectSingleNode("ItemCookTime").InnerText);
                            Qd.Cat = Convert.ToInt32(xn.SelectSingleNode("ItemCategory").InnerText);
                        }
                        Qd.TimeOfPrepCount++;
                        int StartTime= Math.Max(Convert.ToInt32(xn.SelectSingleNode("OrderFirstDisplayedTime").InnerText),0);
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
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //Ws.Cells[2, 1] = "Критерии";
            Ws.Name = "Avg time";


            Ws.Cells[1, 1] = "Баркод";
            Ws.Cells[1, 2] = "Имя";
            Ws.Cells[1, 3] = "Категория";
            Ws.Cells[1, 4] = "Норматив, сек";
            Ws.Cells[1, 5] = "Ср. время, сек";
            int row = 2;
            foreach (QSRDishInfo qd in Tmp)
            {
                if ((qd.Cat == 999)|| (qd.Cat == 700) || (qd.Cat == 101)) continue;
                Ws.Cells[row, 1] = qd.BarCode;
                Ws.Cells[row, 2] = qd.Name;
                Ws.Cells[row, 3] = qd.Cat;
                Ws.Cells[row, 4] = qd.Norm;
                Ws.Cells[row, 5] = qd.TimeOfPrepAvg;
                Ws.Cells[row, 6] = ((double)qd.WrongCount/(double)qd.TimeOfPrepCount);
                
                row++;
            }
        }
    }
    public class QSRDishInfo
    {
        public int BarCode { set; get; }
        public string Name { set; get; }
        public int Norm { set; get; }
        public int Cat { set; get; }
        public int TimeOfPrepSumm { set; get; }
        public int TimeOfPrepCount { set; get; }
        public int WrongCount { set; get; }
        public int WrongSumm { set; get; }
        public double TimeOfPrepAvg
        {
            get
            {
                return (double)TimeOfPrepSumm / (double)TimeOfPrepCount;
            }
        }

    }
}
