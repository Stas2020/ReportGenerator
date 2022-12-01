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
    public static class сRashMatGroups
    {
        static public void GetByYear(int Year, int groupNum)
        {
            //Double Summ = 0;
            //int ChCount = 0;
            //for (int Month = 1; Month < 11; Month++)
            //{
            //    DateTime Fdt = new DateTime(Year, Month, 1);
            //    DateTime Edt = new DateTime(Year, Month + 1, 1);
            //    List<RashMaterials> Tmp = GetRashMat(Fdt, Edt);
            //    Summ += Tmp.Sum(a => a.Value);
            //    ChCount += (Int32)Tmp.Sum(a => a.Checks);
            //}
            //double res = Summ / ChCount;
            Double Consumables = 0;
            Double Proceeds = 0;
            for (int Month = 1; Month < 11; Month++)
            {
                DateTime Fdt = new DateTime(Year, Month, 1);
                DateTime Edt = new DateTime(Year, Month + 1, 1);
                List<RashMaterials> Tmp = GetRashMat(Fdt, Edt, groupNum);
                Consumables += Tmp.Sum(a => a.Consumables);
                Proceeds += Tmp.Sum(a => a.Proceeds);
            }
            double res = Consumables / Proceeds;
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
                if (Dii.Number != 320) continue;
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


        static public List<RashMaterials> GetRashMat(DateTime Fdt, DateTime Edt, int groupNum)
        {
            List<RashMaterials> Tmp = new List<RashMaterials>();


            vfiliasut8.ExchangePeskovFotoGallery srv = new vfiliasut8.ExchangePeskovFotoGallery();
            NetworkCredential Cred = new NetworkCredential("ws", "ws1", "");

            srv.Credentials = Cred;


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            TimeOfPrep.Ges3ServicesObj PrepSrv = new TimeOfPrep.Ges3ServicesObjClient();

            //Dictionary<int, int> ChecksCount = GetChkCount(Fdt);

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

                double consumables = 0;
                try
                {
                    //consumables = srv.getCosts(BuhDepNum.ToString(), Fdt, Edt);
                    string ret = srv.getCostsGroup(BuhDepNum.ToString(), Fdt, Edt);
                    string[] results = ret.Replace("[", "").Replace("]", "").Trim().Split(',');
                    double group1 = 0.0, group2 = 0.0;
                    Double.TryParse(results[0].Replace(".", ","), out group1);
                    Double.TryParse(results[1].Replace(".", ","), out group2);
                    consumables = (groupNum == 1) ? group1 : group2;
                }
                catch
                {

                }
                int res2 = 0;
                //if (ChecksCount.TryGetValue(Dii.Number, out res2))
                //{
                //    Console.WriteLine(Dii.Number.ToString() + " - " + s.ToString() + " - " + ChecksCount[Dii.Number]);
                //}

                double proceeds = 0;
                for (DateTime dt = new DateTime(Fdt.Ticks); dt <= Edt; dt = dt.AddDays(1))
                {
                    Ges3.GestoriCashByDay_T_cashRow[] cash;
                    cl.GestoriCashByDay(Edt, false,
                        new Ges3.GestoriCashByDay_T_shopsRow[] { new Ges3.GestoriCashByDay_T_shopsRow() { codShop = Dii.Number } },
                        out cash);
                    proceeds += cash.Select(_cash => ((double)_cash.sum_nal + (double)_cash.sum_plast)).Sum();
                }

                try
                {
                    RashMaterials RM = new RashMaterials()
                    {
                        Dep = Dii.Number,
                        Consumables = consumables,
                        Proceeds = proceeds,
                        DepName = Dii.Name,
                        //Checks = ChecksCount[Dii.Number]
                    };
                    Tmp.Add(RM);
                }
                catch
                { }
            }

            return Tmp;

        }

        //
        public static List<RashMaterials> GetRashMatByDay(DateTime Fdt, DateTime Edt, int groupNum)
        {
            List<RashMaterials> rashMaterialsList = new List<RashMaterials>();
            vfiliasut8.ExchangePeskovFotoGallery peskovFotoGallery = new vfiliasut8.ExchangePeskovFotoGallery();
            NetworkCredential networkCredential = new NetworkCredential("ws", "ws1", "");
            peskovFotoGallery.Credentials = (ICredentials)networkCredential;
            S2010.DepartmentInfo[] pointList3 = new S2010.XrepSoapClient().GetPointList3();
            ReportMonthResultGenerator.TimeOfPrep.Ges3ServicesObjClient servicesObjClient1 = new ReportMonthResultGenerator.TimeOfPrep.Ges3ServicesObjClient();

            foreach (S2010.DepartmentInfo departmentInfo in pointList3)
            {
                if (departmentInfo.Enabled)
                {
                    ReportMonthResultGenerator.TimeOfPrep.ShopsGoodTime_T_goodtimeRow[] timeTGoodtimeRowArray = new ReportMonthResultGenerator.TimeOfPrep.ShopsGoodTime_T_goodtimeRow[50];
                    int number = departmentInfo.Number;
                    ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient servicesObjClient2 = new ReportMonthResultGenerator.Ges3.Ges3ServicesObjClient();

                    if (departmentInfo.Number != 200)// && departmentInfo.Number != 310)
                    {
                        int? cod_virt_shop;
                        servicesObjClient2.obj_virt(new int?(departmentInfo.Number), out cod_virt_shop);
                        number = cod_virt_shop.Value;
                    }
                    double consumables = 0.0;
                    try
                    {
                        //consumables = peskovFotoGallery.getCosts(number.ToString(), Fdt, Edt);
                        string ret = peskovFotoGallery.getCostsGroup(number.ToString(), Fdt, Edt);
                        string[] results = ret.Replace("[", "").Replace("]", "").Trim().Split(',');
                        double group1 = 0.0, group2 = 0.0;
                        Double.TryParse(results[0].Replace(".", ","), out group1);
                        Double.TryParse(results[1].Replace(".", ","), out group2);
                        consumables = (groupNum == 1) ? group1 : group2;
                    }
                    catch (Exception ex)
                    {
                        ;
                    }

                    double proceeds = 0;
                    Ges3.GestoriSaleshByDay_T_cashRow[] cashRow = null;

                    // Этот сервис весьма глючит ToDo
                    for (int i = 0; i < 10; i++)
                    {
                        if (cashRow != null)
                            continue;
                        try
                        {
                            Ges3.Ges3ServicesObjClient client = new Ges3.Ges3ServicesObjClient();
                            client.GestoriSaleshByDay(Fdt, false, new Ges3.GestoriSaleshByDay_T_shopsRow[] { new Ges3.GestoriSaleshByDay_T_shopsRow() { codShop = departmentInfo.Number } },
                                out cashRow);
                        }
                        catch { cashRow = null; }
                        if (cashRow == null)
                        {
                            Utils.ToDebugLog($"Now:{DateTime.Now:HH:mm:ss}  ОШИБКА В TRY   ForDate:{Edt:dd.MM.yyyy}\t" + departmentInfo.Number.ToString() + "\t" + consumables.ToString() + "\t" + proceeds.ToString(), true);
                            System.Threading.Thread.Sleep(3500);
                        }
                    }
                    if (cashRow == null)
                    {
                        Ges3.Ges3ServicesObjClient client = new Ges3.Ges3ServicesObjClient();
                        client.GestoriSaleshByDay(Fdt, false, new Ges3.GestoriSaleshByDay_T_shopsRow[] { new Ges3.GestoriSaleshByDay_T_shopsRow() { codShop = departmentInfo.Number } },
                            out cashRow);
                    }
                    proceeds = cashRow.Sum(_row => (double)((_row.sum_nal ?? 0) + (_row.sum_plast ?? 0) + (_row.SUM_rx ?? 0)));

                    //var proceedsTEST = cashRow.Sum(_row => (double)((_row.sum_nal ?? 0) + (_row.sum_plast ?? 0)));




                    //if (checkCount.TryGetValue(departmentInfo.Number, out num2))
                    //    Console.WriteLine(departmentInfo.Number.ToString() + " - " + consumables.ToString() + " - " + checkCount[departmentInfo.Number].ToString());

                    Utils.ToDebugLog($"Now:{DateTime.Now:HH:mm:ss}  ForDate:{Edt:dd.MM.yyyy}\t" + departmentInfo.Number.ToString() + "\t" + consumables.ToString() + "\t" + proceeds.ToString());

                    try
                    {
                        RashMaterials rashMaterials = new RashMaterials()
                        {
                            Dep = departmentInfo.Number,
                            DepName = departmentInfo.Name,
                            Consumables = consumables,
                            Proceeds = proceeds
                            //Value = num1,                            
                            //Checks = (double)checkCount[departmentInfo.Number]
                        };
                        rashMaterialsList.Add(rashMaterials);
                    }
                    catch
                    {
                    }
                }
            }
            return rashMaterialsList;
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

