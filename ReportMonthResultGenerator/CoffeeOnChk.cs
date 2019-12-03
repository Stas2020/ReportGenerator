using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
    using System.Data;
    using System.Data.Sql;
    using System.Data.SqlClient;


namespace ReportMonthResultGenerator
{
    class CoffeeOnChk
    {

        
        public static List<ReportMonthResult> GetCoffeeOnChk(DateTime Month)
        {
            List<ReportMonthResult> Tmp = new List<ReportMonthResult> ();
      //      string CoffeeToGo="";

            int MonthNumber = Month.Month;
            int YearNumber = Month.Year;

            /*
            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            conn1.Open();
            string q1 = "SELECT  [Number] FROM [dbo].[Item] where [FK_SalesCategory] = ( SELECT [Id] FROM [dbo].[Category] where Number =38) ";

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc1 = new SqlCommand(q1, conn1);

            SqlDataReader Sr1 = Sc1.ExecuteReader();
            
            while (Sr1.Read())
            {
                CoffeeToGo+=(Sr1.GetValue(0).ToString())+",";
            }

            CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length-1);

            conn1.Close();

    */
            string CoffeeToGo =  String.Join(", ", CubeData.GetDishOfSalesCat(38).ToArray());
            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            
            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]) FROM [dbo].[Продажи]  where  [Год]=@year and [Месяц]=@Month  and [СуммаИтог]!=0 " +
" and [БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] in " +
"  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=821 and [cod_sgr]<=823))  and [НомерКлиента]!=6 and [НомерКлиента]!=54 "+
 "  and ([Стол]<200 or [Стол]>254) "+
//"  and (not ([Стол]>=200 and [Стол]<=209) or ([Стол]>=235 and [Стол]<=240) or ([Стол]>=241 and [Стол]<=245) )" +
" and not [БарКод] in (" + CoffeeToGo + ")  group by [КодПодразд]";
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
  //"  and (not ([Стол]>=200 and [Стол]<=209) or ([Стол]>=235 and [Стол]<=240) or ([Стол]>=241 and [Стол]<=245) )" +
   " and  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 )  and   [НомерКлиента]!=6 and [НомерКлиента]!=54 group by  [КодПодразд] ";


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
    public class CoffOnCh
    {
        public int CoffeeCount = 0;
        public int Dep = 0;
        public int ChkCount = 0;
    }
}
