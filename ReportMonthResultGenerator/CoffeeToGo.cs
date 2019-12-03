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
    class CoffeeToGo
    {
        public static void GetCoffeeTogo(DateTime Month)
        {
            List<ReportMonthResult> Tmp = new List<ReportMonthResult>();
            string CoffeeToGo = "";

            int MonthNumber = Month.Month;
            int YearNumber = Month.Year;

            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            conn1.Open();
            string q1 = "SELECT  [Number] FROM [dbo].[Item] where [FK_SalesCategory] = ( SELECT [Id] FROM [dbo].[Category] where Number =38) ";

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc1 = new SqlCommand(q1, conn1);

            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
            }

            CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);

            conn1.Close();

            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]) FROM [dbo].[Продажи]  where  [Год]=@year and [Месяц]=@Month  and [СуммаИтог]!=0 and [Колич]<100 " +
" and ([БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] in " +
"  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=821 and [cod_sgr]<=823)  )  and ([НомерКлиента]=6 or [НомерКлиента]=54) " +
" or [БарКод] in (" + CoffeeToGo + "))  group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();
            List<int> CoffCount = new List<int>();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Кофе с собой";
            Worksheet Ws2 = Wb.Worksheets.Add();
            Ws2.Name = "Не кофе с собой";

            Ws.Cells[1, 1] = "Подразделение";
            Ws.Cells[1, 2] = "Чашки";

            Ws2.Cells[1, 1] = "Подразделение";
            Ws2.Cells[1, 2] = "Штуки";

            int row = 2;
            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                ReportMonthResult Mr = new ReportMonthResult()
                {

                    Department = Dep,
                    //Value = Sr.GetInt32(1),
                    Value2 = Sr.GetInt32(1),
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            foreach (S2010.DepartmentInfo Dep in DepList)
            {
                try
                {
                    Ws.Cells[row, 1] = Dep.Name;
                    Ws.Cells[row, 2] = Tmp.Where(a => a.Department == Dep.Number).First().Value2;

                    row++;
                }
                catch { }
                
            }


            q = "SELECT " +
"[КодПодразд], sum([Колич]) FROM [dbo].[Продажи]  where  [Год]=@year and [Месяц]=@Month  and [СуммаИтог]!=0 and [Колич]<100 " +
" and ([БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] not in " +
"  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=821 and [cod_sgr]<=823)  )  and ([НомерКлиента]=6 or [НомерКлиента]=54) " +
" )  group by [КодПодразд]";


            Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            P1 = new SqlParameter("year", Month.Year);
            P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            Sr = Sc.ExecuteReader();
  
            row = 2;
            Tmp = new List<ReportMonthResult>();
            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                ReportMonthResult Mr = new ReportMonthResult()
                {

                    Department = Dep,
                    //Value = Sr.GetInt32(1),
                    Value2 = Sr.GetInt32(1),
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            foreach (S2010.DepartmentInfo Dep in DepList)
            {
                try
                {
                    Ws2.Cells[row, 1] = Dep.Name;
                    Ws2.Cells[row, 2] = Tmp.Where(a => a.Department == Dep.Number).First().Value2;

                    row++;
                }
                catch { }

            }
  
        }
    }
}
