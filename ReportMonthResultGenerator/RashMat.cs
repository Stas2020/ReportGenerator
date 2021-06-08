using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using System.Data.Sql;
using System.Xml;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using ReportMonthResultGenerator.AutoCalc;

namespace ReportMonthResultGenerator
{
    public static class сRashMat
    {
        static public void GetByYear(int Year)
        {
            Double Summ = 0;
            int ChCount = 0;
            for (int Month = 1; Month < 11; Month++)
            {
                DateTime Fdt = new DateTime(Year, Month, 1);
                DateTime Edt = new DateTime(Year, Month + 1, 1);
                List<RashMaterials> Tmp = GetRashMat(Fdt, Edt);
                Summ += Tmp.Sum(a => a.Value);
                ChCount += (Int32)Tmp.Sum(a => a.Checks);
            }
            double res = Summ / ChCount;
            Console.WriteLine(res);


        }


        static public List<RashMaterials> GetRashMatList(DateTime Fdt, DateTime Edt)
        {

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;


            List<RashMaterials> Tmp = new List<RashMaterials>();


            vfiliasut8.ExchangePeskovFotoGallery srv = new vfiliasut8.ExchangePeskovFotoGallery();
            NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");

            srv.Credentials = Cred;


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            TimeOfPrep.Ges3ServicesObj PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

         //   Dictionary<int, int> ChecksCount = GetChkCount(Fdt);

            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                if (Dii.Number!=320) continue;
                //
                TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                TimeOfPrep.ShopsGoodTimeRequest req = new TimeOfPrep.ShopsGoodTimeRequest();
                req.DepId = Dii.Number;
                req.dateStart = Fdt;
                req.dateEnd = Edt.AddDays(-1);
                // TimeOfPrep.ShopsGoodTimeResponse resp =  PrepSrv.ShopsGoodTime(req);
                //  int ChecksCount = resp.kol_ch.Value;


                //PrepSrv.sh ShopsGoodTime(Dii.Number, Fdt, Edt.AddDays(-1), out res);
                //res.kol_ch

                int BuhDepNum = Dii.Number;

                Ges3.Ges3ServicesObjClient cl = new Ges3.Ges3ServicesObjClient();

                if ((Dii.Number != 200) && (Dii.Number != 310))
                {
                    int? r;
                    cl.obj_virt(Dii.Number, out r);
                    BuhDepNum = r.Value;
                }

                string s = srv.getCostsList(BuhDepNum.ToString(), Fdt, Edt);

                XmlDocument Doc = new XmlDocument();
                Doc.LoadXml(s);
                XmlNodeList Xn = Doc.GetElementsByTagName("row");
                int row = 2;
                foreach (XmlNode Xnn in Xn)
                {
                    int col = 4;
                    foreach (XmlNode Xnnn in Xnn.ChildNodes)
                    {
                        
                        Ws.Cells[row, col] = Xnnn.InnerText;
                        col--;
                        
                        
                    }
                    row++;
                }


                /*
                Console.WriteLine(s);

                int res2 = 0;
                if (ChecksCount.TryGetValue(Dii.Number, out res2))
                {
                    Console.WriteLine(Dii.Number.ToString() + " - " + s.ToString() + " - " + ChecksCount[Dii.Number]);
                }
                */
                //double s = srv.g


            
            }

            return Tmp;

        }


        static public List<RashMaterials> GetRashMat(DateTime Fdt, DateTime Edt)
        {
            List<RashMaterials> Tmp = new List<RashMaterials>();


            vfiliasut8.ExchangePeskovFotoGallery srv = new vfiliasut8.ExchangePeskovFotoGallery();
            NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");

            srv.Credentials = Cred;


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            TimeOfPrep.Ges3ServicesObj PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

            Dictionary<int, int> ChecksCount = GetChkCount(Fdt);

            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
              
                TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                TimeOfPrep.ShopsGoodTimeRequest req = new TimeOfPrep.ShopsGoodTimeRequest();
                req.DepId = Dii.Number;
                req.dateStart = Fdt;
                req.dateEnd = Edt.AddDays(-1);
               

                int BuhDepNum = Dii.Number;

                Ges3.Ges3ServicesObjClient cl = new Ges3.Ges3ServicesObjClient();

                if ((Dii.Number != 200) && (Dii.Number != 310))
                {
                    int? r;
                    cl.obj_virt(Dii.Number, out r);
                    BuhDepNum = r.Value;
                }
               
                double s = 0;
                try
                {
                    s = srv.getCosts(BuhDepNum.ToString(), Fdt, Edt);
                }
                catch
                {

                }
                int res2 = 0;
                if (ChecksCount.TryGetValue(Dii.Number, out res2))
                {
                    Console.WriteLine(Dii.Number.ToString() + " - " + s.ToString() + " - " + ChecksCount[Dii.Number]);
                }
               
                try
                {
                    RashMaterials RM = new RashMaterials()
                    {
                        Dep = Dii.Number,
                        Value = s,
                        DepName = Dii.Name,

                        Checks = ChecksCount[Dii.Number]

                    };
                    Tmp.Add(RM);
                }
                catch
                { }
            }

            return Tmp;

        }


        static public List<RashMaterials> GetRashMatByDay(DateTime Fdt, DateTime Edt)
        {
            List<RashMaterials> Tmp = new List<RashMaterials>();


            vfiliasut8.ExchangePeskovFotoGallery srv = new vfiliasut8.ExchangePeskovFotoGallery();
            NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");

            srv.Credentials = Cred;


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            TimeOfPrep.Ges3ServicesObj PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

            //Dictionary<int, int> ChecksCount = GetChkCountByDay(Fdt);
            Dictionary<int, int> ChecksCount = CheckCountSingletone.Instance.AllCheckCount.GetCheckCount(Fdt);

            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                //if ((Dii.Number!=111)&& (Dii.Number != 121))  continue;
                TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] res = new TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                int BuhDepNum = Dii.Number;

                Ges3.Ges3ServicesObjClient cl = new Ges3.Ges3ServicesObjClient();

                if ((Dii.Number != 200) && (Dii.Number != 310))
                {
                    int? r;
                    try
                    {
                        cl.obj_virt(Dii.Number, out r);
                    }
                    catch(Exception ex)
                    {
                        Utils.ToLog($"Не удалось получить бухгалтерский номер участка {Dii.Number}. Метод Ges3Services.obj_virt. Сообщение об ошибке: {ex.Message}", true);
                        continue;
                    }
                    BuhDepNum = r.Value;
                }

                double s = 0;
                try
                {
                    s = srv.getCosts(BuhDepNum.ToString(), Fdt, Edt);
                }
                catch
                {

                }
                int res2 = 0;
                if (ChecksCount.TryGetValue(Dii.Number, out res2))
                {
                    Console.WriteLine(Dii.Number.ToString() + " - " + s.ToString() + " - " + ChecksCount[Dii.Number]);
                }

                try
                {
                    RashMaterials RM = new RashMaterials()
                    {
                        Dep = Dii.Number,
                        Value = s,
                        DepName = Dii.Name,

                        Checks = ChecksCount[Dii.Number]

                    };
                    Tmp.Add(RM);
                }
                catch
                { }
            }

            return Tmp;

        }

        public static Dictionary<int, int> GetChkCountByDay(DateTime Day)
        {
            Dictionary<int, int> Tmp = new Dictionary<int, int>();
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();


            string q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Vall]  where [Год]=@P1 and [Месяц]=@P2 and [Число]=@P3  and [НомерСтроки]=1 " +
   " group by  [КодПодразд] ";


            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("P1", Day.Year);
            SqlParameter P2 = new SqlParameter("P2", Day.Month);
            SqlParameter P3 = new SqlParameter("P3", Day.Day);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.Parameters.Add(P3);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();
            List<int> ChkCount = new List<int>();
            while (Sr2.Read())
            {
                try
                {
                    Tmp.Add(Sr2.GetInt32(0), Sr2.GetInt32(1));

                }
                catch
                { }

            }
            Sr2.Close();


            conn.Close();



            return Tmp;
        }

        public static Dictionary<int, int> GetChkCount(DateTime Month)
        {
            Dictionary<int, int> Tmp = new Dictionary<int, int>();
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();


            string q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Vall]  where [Год]=@P1 and [Месяц]=@P2  and [НомерСтроки]=1 " +
   " group by  [КодПодразд] ";


            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("P1", Month.Year);
            SqlParameter P2 = new SqlParameter("P2", Month.Month);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();
            List<int> ChkCount = new List<int>();
            while (Sr2.Read())
            {
                try
                {
                    Tmp.Add(Sr2.GetInt32(0), Sr2.GetInt32(1));

                }
                catch
                { }

            }
            Sr2.Close();


            conn.Close();



            return Tmp;
        }

    }
}

