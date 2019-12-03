using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;


namespace ReportMonthResultGenerator
{
    public class DesertSale
    {
        public DesertSale()
        { }
        public DateTime dt {set;get;}
        public int barcode {set;get;}
        public int count { set; get; }

    }

    public static class DesertsOnChk
    {

        public static void  GetDesertsSaleReport(DateTime StartDate, DateTime StopDate, int Dep)
        {
            int NightHours=6;
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible=true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            int Collumn = 3;
         
            for (DateTime dt = StartDate; dt <= StopDate; dt = dt.AddDays(1))
            {
                Ws.Cells[1, Collumn] = dt.ToString(@"dd/MM/yyyy");
                Collumn++;
            }
            List<DesertSale> DesSel = GetDesertsSales(StartDate, StopDate, Dep);
            DesSel.Sort((a, b) => (a.barcode.CompareTo(b.barcode)));
            int Row = 2;
             foreach (int  BarCode in  DesSel.Select(a=>a.barcode).Distinct())
             {
                 
                 Ws.Cells[Row, 1] = BarCode;
                 Ws.Cells[Row, 2] = DesertsOnStop.GetDishName(BarCode);
                 Collumn = 3;
                 for (DateTime dt = StartDate; dt <= StopDate; dt = dt.AddDays(1))
                 {
                     try
                     {
                         int SaleCount = DesSel.Where(a => a.barcode == BarCode && a.dt >= dt.AddHours(NightHours) && a.dt < dt.AddDays(1).AddHours(NightHours)).Sum(b => b.count);
                         Ws.Cells[Row, Collumn] = SaleCount;
                     }
                     catch
                     { }
                     Collumn++;
                 }
                 Row++;
             
             }
        
        }
        

        public static List<DesertSale> GetDesertsSales(DateTime StartDate, DateTime StopDate, int Dep)
        {
            List<DesertSale> Tmp = new List<DesertSale> ();
            string Deserts = "";
            foreach (int d in DesertsOnStop.GetDesertList(Dep))
            {
                Deserts += d.ToString() + ",";
            }
            Deserts = Deserts.Substring(0, Deserts.Length - 1);
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[Баркод], [Колич],[Год],[Месяц],[Число],[Час] FROM [dbo].[Продажи]  where  [Год]=@year and [Месяц]=@Month and [Число]>=@Day1 and [Число]<=@Day2+1 and [СуммаИтог]!=0 and [КодПодразд]=@Dep and [Колич]<10" +
" and [БарКод]  in    (" + Deserts + ") ";



            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", StartDate.Year);
            SqlParameter P2 = new SqlParameter("Month", StartDate.Month);
            SqlParameter P3 = new SqlParameter("Day1", StartDate.Day);
            SqlParameter P4 = new SqlParameter("Day2", StopDate.Day);
            SqlParameter P5 = new SqlParameter("Dep", Dep);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);
            Sc.Parameters.Add(P3);
            Sc.Parameters.Add(P4);
            Sc.Parameters.Add(P5);

            SqlDataReader Sr = Sc.ExecuteReader();
            List<int> CoffCount = new List<int>();


            while (Sr.Read())
            {
                DesertSale Ds = new DesertSale()
                {
                    barcode = Sr.GetInt32(0),
                    count = Sr.GetInt32(1),
                    dt= new DateTime (Sr.GetInt32 (2),Sr.GetInt32 (3),Sr.GetInt32 (4),Sr.GetInt32 (5),0,0)
                };
                Tmp.Add(Ds);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();
            return Tmp;
        }



        public static List<ReportMonthResult> GetDesertsOnChk(DateTime Month)
        {

            List<ReportMonthResult> Tmp = new List<ReportMonthResult>();


            int MonthNumber = Month.Month;
            int YearNumber = Month.Year;
            
                        string Deserts = "";

                      //  SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
                     //   conn1.Open();


                        foreach (int d in DesertsOnStop.GetDesertList(0))
                        {
                            Deserts += d.ToString() + ",";
                        }
                            Deserts = Deserts.Substring(0, Deserts.Length - 1);
            /*
                        string CoffeeToGo = "";



                      string  q1 = "SELECT  [Number] FROM [dbo].[Item] where [FK_SalesCategory] = ( SELECT [Id] FROM [dbo].[Category] where Number =38) ";

                        //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

                        SqlCommand Sc1 = new SqlCommand(q1, conn1);

                        SqlDataReader Sr1 = Sc1.ExecuteReader();

                        while (Sr1.Read())
                        {
                            CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
                        }

                        CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);




                        conn1.Close();
                        */
            string CoffeeToGo = String.Join(", ",CubeData.GetDishOfSalesCat(38).ToArray());

            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]) FROM [dbo].[Продажи]  where  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and [Колич]<50" +
" and [БарКод]  in    (" + Deserts + ") " +
"  and ([Стол]<200 or [Стол]>254) "+
//"  and (not ([Стол]>=200 and [Стол]<=209) or ([Стол]>=235 and [Стол]<=240) or ([Стол]>=241 and [Стол]<=245) ) " +
"  group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();
            List<int> CoffCount = new List<int>();


            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                ReportMonthResult Mr = new ReportMonthResult()
                {

                    Department = Dep,
                    //Value = Sr.GetInt32(1),
                    Value2 = (double)Sr.GetDecimal(1),
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();
            //   q = "SELECT distinct [КодПодр] ,count(*) FROM [dbo].[выручка] where  [Год]=@year  and [Месяц]=@month  and [СуммаИтогРуб]>0 and ([НомерКлиента]!=6 and [НомерКлиента]!=54 ) and [НомерОфицианта]!=9267 group by [КодПодр]  ";
            q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Продажи]  where [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and [НомерСтроки]=1 " +
 " and [НомерЧека]  in ( select ([НомерЧека] ) FROM [dbo].[Продажи] b where [БарКод] not in " +
  " ( " + CoffeeToGo + " ) " +
  "  and ([Стол]<200 or [Стол]>254) " +
  //"  and (not ([Стол]>=200 and [Стол]<=209) or ([Стол]>=235 and [Стол]<=240) or ([Стол]>=241 and [Стол]<=245) ) " +
   " and  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 )  " +
   //         " and   [НомерКлиента]!=6 and [НомерКлиента]!=54 and [НомерКлиента]!=1 "+
            " group by  [КодПодразд] ";


            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc2 = new SqlCommand(q, conn);
            P1 = new SqlParameter("year", Month.Year);
            P2 = new SqlParameter("Month", Month.Month);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();
            List<int> ChkCount = new List<int>();
            while (Sr2.Read())
            {
                try
                {
                    int Dep = Sr2.GetInt32(0);
                    Tmp.FirstOrDefault(a => a.Department == Dep).Value = Sr2.GetInt32(1);
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
