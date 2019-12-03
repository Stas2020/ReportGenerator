using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Globalization;

namespace ReportMonthResultGenerator
{
    static class ReportsDN
    {


        internal static void CalcEffect()
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            Dictionary<int, double> moneys = new Dictionary<int, double>() { { 2019, 0 }, { 2018, 0 }, { 2017, 0 }, { 2016, 0 } };
            Dictionary<int, double> hours = new Dictionary<int, double>() { { 2019, 0 }, { 2018, 0 }, { 2017, 0 }, { 2016, 0 } };
            foreach (int y in moneys.Keys)
            {
                var d1 = new DateTime(y, 3, 1);
                var CityDeps = DepList.Where(b => b.Place.Trim() == "Город" && b.Enabled  && b.Number!=190 && b.Number != 191 && b.Number != 450 && b.Number != 451 && b.Number != 180 && b.Number != 280).Select(b => b.Number);
              var  m = (double)CubeData.GetMoneyCount(d1).Where(a => CityDeps.Contains(a.Key)).Sum(a => a.Value);
                var h = (double)StaffWtToExcel.GetWtsNoWaiter(d1, d1.AddMonths(1)).Where(a => CityDeps.Contains(a.Key)).Sum(a => a.Value);
                Console.WriteLine($"{y}: {m / h }");

            }
            Console.WriteLine($"airs");
            foreach (int y in moneys.Keys)
            {
                var d1 = new DateTime(y, 3, 1);
                var CityDeps = DepList.Where(b => b.Place.Trim() != "Город" && b.Enabled && b.Number != 190 && b.Number != 191).Select(b => b.Number);
                var m = (double)CubeData.GetMoneyCount(d1).Where(a => CityDeps.Contains(a.Key)).Sum(a => a.Value);
                var h = (double)StaffWtToExcel.GetWtsNoWaiter(d1, d1.AddMonths(1)).Where(a => CityDeps.Contains(a.Key)).Sum(a => a.Value);
                Console.WriteLine($"{y}: {m / h }");

            }
            Console.ReadKey();
        }





        internal static void GetEffectByDayRange()
        {
            var d1 = new DateTime(2019, 3, 30);
            var d2 = new DateTime(2019, 3, 31);
            var MoneyCount = MergeDepo(CubeData.GetMoneyCountByDay(d1));
            
            for (DateTime dt = d1.AddDays(1); dt <= d2; dt = dt.AddDays(1))
            {
                var MoneyCountTmp = MergeDepo(CubeData.GetMoneyCountByDay(dt));
                foreach (int dep in MoneyCountTmp.Keys)
                {
                    decimal m = 0;
                    if (!MoneyCount.TryGetValue(dep, out m))
                    {
                        MoneyCount.Add(dep, MoneyCountTmp[dep]);
                    }
                    else
                    {
                        MoneyCount[dep] += MoneyCountTmp[dep];
                    }
                }
         
                //Раб часы
                
            }
            var wts = MergeDepo(StaffWtToExcel.GetWtsNoWaiter(d1, d2.AddDays(1)));

            List<int> deps = new List<int>() { 190, 255, 375 };

            foreach (int d in deps)
            {
                try
                {
                    Console.WriteLine($"{d}: {MoneyCount[d]/ wts[d]}");
                }
                catch
                {
                }
            }
            Console.ReadKey();

        }
        private static Dictionary<int, decimal> MergeDepo(Dictionary<int, decimal> indata)
        {
            decimal m = 0;
            if (indata.TryGetValue(191,out m))
            {
                decimal m2 = 0;
                if (indata.TryGetValue(190, out m2))
                {
                    indata[190] += m;
                }
                else
                {
                    indata.Add(190, m);
                }

            }
            return indata;
        }
            

        internal static void GenReportsFOT()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            string FPath = @"D:\ДН\фот.xlsx";
            Application app2 = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook Wb2 = app2.Workbooks.Open(FPath);
            Microsoft.Office.Interop.Excel.Worksheet Ws2 = Wb2.Worksheets[2];

            List<int> DepsAir = new List<int>() {210,211,212, 213, 214, 216, 217, 222, 230, 231, 300 ,240};
            List<int> DepsCityCoff = new List<int>() { 101, 104, 130, 177, 205, 260, 270, 295, 310, 370, 371, 375, 380, 390, 395, 290, 285,180,255,264 ,200,280,320,350};
            double[] AirFot = new double[13];
            double[] CFot = new double[13];
            Cube2005DataContext data = new Cube2005DataContext();
            //int ReadRow = 1;
            int ReadrowMax = 27797;
            //int ReadrowMax = 10;
            //List<int> Empls = data.StaffEmployeeEx.Where(a => DepsCity.Contains(a.SUBDIVISION_ID.Value) && a.DISMISSAL_DATE == null).Select(a => a.EMPLOYEE_ID).ToList();
            //List<EmplSal> EmplSalData = GetEmpSals(Empls);

            for (int ReadRow = 1; ReadRow < ReadrowMax; ReadRow++)

            {
                try
                {
                    int Month = Convert.ToInt32(Ws2.Cells[ReadRow, 1].value);
                    int Year = Convert.ToInt32(Ws2.Cells[ReadRow, 2].value);
                    if (Year != 2017)
                    {
                        continue;
                    }
                    int Dep = Convert.ToInt32(Ws2.Cells[ReadRow, 3].value);
                    double Cash = Convert.ToDouble(Ws2.Cells[ReadRow, 9].value);

                    if (DepsAir.Contains(Dep))
                    {
                        AirFot[Month] += Cash;
                    }
                    else if (DepsCityCoff.Contains(Dep))
                    {
                        CFot[Month] += Cash;
                    }
                    else
                    {
                        Console.WriteLine("Other Dep " + Dep );
                    }

                }
                catch (Exception e)
                {
                    Utils.ToLog("Error GenReportsFOT " + e.Message);
                }
            }
            CultureInfo ci = new CultureInfo("ru-RU");
            // Get the DateTimeFormatInfo for the en-US culture.
            DateTimeFormatInfo dtfi = ci.DateTimeFormat;
            for (int Month = 1; Month < 13; Month++)
            {
                Ws.Cells[Month+1, 1] = dtfi.MonthNames[Month-1].ToString();
                Ws.Cells[Month + 1, 2] = CFot[Month].ToString("0.00");
                Ws.Cells[Month+1, 3] = AirFot[Month].ToString("0.00");

            }

        }


        internal static void GenReportsPartSales()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            //Ws.Cells[2, 1] = "Критерии";
            Ws.Name = "Напитки в чеке";


            Ws.Cells[1, 1] = "Доля кухни"; //9,10,11,12,13
            Ws.Cells[2, 1] = "Доля стойки";
            Ws.Cells[3, 1] = "Доля десертов";
            Ws.Cells[4, 1] = "Доля горячих блюд";
            Ws.Cells[5, 1] = "Доля холодных блюд";
            Ws.Cells[6, 1] = "Доля кофейных напитков";
            Ws.Cells[7, 1] = "Доля не кофейных напитков";
            Ws.Cells[8, 1] = "Доля крепкого алкоголя";
            Ws.Cells[9, 1] = "Доля пива";
            Ws.Cells[10, 1] = "Доля вина";

            DateTime Month = new DateTime();
            List<ReportMonthResult> mCoffeeOnChk = new List<ReportMonthResult>();
            DateTime StopDt = new DateTime(2017, 6, 13);
            DateTime StartDt = new DateTime(2017, 5, 01);
            string CityDeps = "101,104,130,177,180,200,205,255,260,264,270,295,310,370,371,375,380,390,395,280,290,285";
            //string CityDeps = "230,300,240,212,213,216,217,231,222";

            decimal AllSales = GetSalesByDeps(StartDt, StopDt, CityDeps);
            //  decimal v1 = GetSalesByDepsAndBc( StartDt, StopDt, CityDeps,GetBarCodesOfSalesGroup(41));


            try
            {
                /*
                Worksheet Ws2= Wb.Worksheets.Add();
                Ws2.Name = "Кухня";
                Ws.Cells[1, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("9,10,11,12,13,14,15,16,23,983,984,987", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Стойка";
                Ws.Cells[2, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("1,4,5,6,7,17,18,19,20,21,22,37,38,981,982,985,986", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Десерты";
                Ws.Cells[3, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("6,982", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Горячее";
                Ws.Cells[4, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("11,12,14,15,16,23,13,984,987 ", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Холодное";
                Ws.Cells[5, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("10,983,9", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Кофе";
                Ws.Cells[6, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("1,17", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Не кофе";
                Ws.Cells[7, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("4,5,981", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Крепкий алкоголь";
                Ws.Cells[8, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("19", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Пиво";
                Ws.Cells[9, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("21", Ws2)) / AllSales;
                Ws2 = Wb.Worksheets.Add();
                Ws2.Name = "Вино";
                Ws.Cells[10, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroupsDom("20", Ws2)) / AllSales;
                */

                Ws.Cells[1, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(41)) / AllSales;
                Ws.Cells[2, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(44)) / AllSales;
                Ws.Cells[3, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(54)) / AllSales;
                Ws.Cells[4, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(58)) / AllSales;
                Ws.Cells[5, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(57)) / AllSales;
                Ws.Cells[6, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(49)) / AllSales;
                Ws.Cells[7, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, CubeData.GetDishOfGroup(48)) / AllSales;
                Ws.Cells[8, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroup(19)) / AllSales;
                Ws.Cells[9, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroup(21)) / AllSales;
                Ws.Cells[10, 2] = GetSalesByDepsAndBc(StartDt, StopDt, CityDeps, GetBarCodesOfSalesGroup(32) + GetBarCodesOfSalesGroup(20)) / AllSales;

            }
            catch
            {

            }



            //  Ws.get_Range("B2:C27").NumberFormat = "%";
            // app.Save(System.Reflection.Missing.Value);

            app = null;
        }


        internal static void GenReports()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //Ws.Cells[2, 1] = "Критерии";
            Ws.Name = "Напитки в чеке";


            Ws.Cells[1, 1] = "Ресторан";
            Ws.Cells[1, 2] = "Доля кофе в чеке";
            Ws.Cells[1, 3] = "Доля вина в чеке";

            DateTime Month = new DateTime();
            List<ReportMonthResult> mCoffeeOnChk = new List<ReportMonthResult>();
            DateTime StopDt = new DateTime(2016, 1, 10);
            DateTime StartDt = StopDt.AddDays(-14);
            mCoffeeOnChk = GetCoffeeAndVineOnChkByMoney(StartDt, StopDt);

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 1;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                Ws.Cells[row, 1] = Dii.Name;


                try
                {
                    ReportMonthResult DesSpis = mCoffeeOnChk.Where(a => a.Department.Value == Dii.Number).First();
                    Ws.Cells[row, 2] = DesSpis.Value2 / DesSpis.Value;
                    Ws.Cells[row, 3] = DesSpis.Value3 / DesSpis.Value;
                    Ws.Cells[row, 4] = DesSpis.Value;
                    Ws.Cells[row, 5] = DesSpis.Value2;
                    Ws.Cells[row, 6] = DesSpis.Value3;

                }
                catch
                {

                }

                row++;
            }
            Ws.get_Range("B2:C27").NumberFormat = "0.00";
            app.Save(System.Reflection.Missing.Value);

            app = null;
        }

        private static string GetBarCodesOfSalesGroupsDom(string Groups)
        {
            return GetBarCodesOfSalesGroupsDom(Groups, null);
        }
        private static string GetBarCodesOfSalesGroupsDom(string Groups, Worksheet Ws)
        {
            string CoffeeToGo = "";
            SqlConnection conn1 = new SqlConnection(@"Data Source=termdom1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=");
            conn1.Open();
            string q1 = "SELECT  [Number] FROM [dbo].[Item] where [FK_SalesCategory] in ( SELECT [Id] FROM [dbo].[Category] where Number in (" + Groups + ")) ";
            SqlCommand Sc1 = new SqlCommand(q1, conn1);
            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
            }

            CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);
            conn1.Close();
            if (Ws != null)
            {
                PrintItemsDom(CoffeeToGo, Ws);
            }
            return CoffeeToGo;

        }


        private static void PrintItemsDom(string Bcs, Worksheet Ws)
        {
            SqlConnection conn1 = new SqlConnection(@"Data Source=termdom1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=sa;Password=");
            conn1.Open();
            string q1 = "SELECT  [Number],[LongName] FROM [dbo].[Item] where [Number] in ( " + Bcs + ") order by [Number]";
            SqlCommand Sc1 = new SqlCommand(q1, conn1);
            SqlDataReader Sr1 = Sc1.ExecuteReader();
            int row = 1;
            while (Sr1.Read())
            {
                Ws.Cells[row, 1] = Sr1.GetValue(0).ToString();
                Ws.Cells[row, 2] = Sr1.GetValue(1).ToString();
                row++;
            }


            conn1.Close();
        }
        private static string GetBarCodesOfSalesGroup(int Gr)
        {
            string CoffeeToGo = "";
            SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            conn1.Open();
            string q1 = "SELECT  [Number] FROM [dbo].[Item] where [FK_SalesCategory] = ( SELECT [Id] FROM [dbo].[Category] where Number =" + Gr.ToString() + ") ";
            SqlCommand Sc1 = new SqlCommand(q1, conn1);
            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
            }

            CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);
            conn1.Close();
            return CoffeeToGo;

        }

        static public decimal GetSalesByDeps(DateTime StartDt, DateTime StopDt, string Deps)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            string q = "SELECT " +
"sum([СуммаИтог]) FROM [dbo].[Продажи]  where  [ДатаВремя]>=@StartDt and [ДатаВремя]<@StopDt  and [СуммаИтог]!=0 and" +
"[КодПодразд] in (" + Deps + ")";

            /*
            " and [БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] in " +
            "  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=821 and [cod_sgr]<=823)  )  and [НомерКлиента]!=6 and [НомерКлиента]!=54 " +
            " and not [БарКод] in (" + CoffeeToGo + ")  group by [КодПодразд]";
            */

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("StartDt", StartDt);
            SqlParameter P2 = new SqlParameter("StopDt", StopDt);
            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();
            List<int> CoffCount = new List<int>();
            decimal res = 0;
            while (Sr.Read())
            {

                res = Sr.GetSqlDecimal(0).Value;

            }
            Sr.Close();
            return res;
        }


        static public decimal GetSalesByDepsAndBc(DateTime StartDt, DateTime StopDt, string Deps, string Bcs)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            string q = "SELECT " +
"sum([СуммаИтог]) FROM [dbo].[Продажи]  where  [ДатаВремя]>=@StartDt and [ДатаВремя]<@StopDt  and [СуммаИтог]!=0 and" +
"[КодПодразд] in (" + Deps + ") and [БарКод]  in (" + Bcs + ")";


            //" and [БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] in " +
            //"  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=821 and [cod_sgr]<=823)  )  ";// +
            //" and not [БарКод] in (" + CoffeeToGo + ")  group by [КодПодразд]";


            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("StartDt", StartDt);
            SqlParameter P2 = new SqlParameter("StopDt", StopDt);
            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();
            List<int> CoffCount = new List<int>();
            decimal res = 0;
            while (Sr.Read())
            {

                res = Sr.GetSqlDecimal(0).Value;

            }
            Sr.Close();
            return res;
        }



        public static List<ReportMonthResult> GetCoffeeAndVineOnChkByMoney(DateTime StartDt, DateTime StopDt)
        {
            List<ReportMonthResult> Tmp = new List<ReportMonthResult>();
            string CoffeeToGo = "";



            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            conn1.Open();
            string q1 = "SELECT  [Number] FROM [dbo].[Item] where [FK_SalesCategory] = ( SELECT [Id] FROM [dbo].[Category] where Number =38) ";
            SqlCommand Sc1 = new SqlCommand(q1, conn1);

            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
            }

            CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);

            conn1.Close();

            //Кофе
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            string q = "SELECT " +
"[КодПодразд], sum([СуммаИтог]) FROM [dbo].[Продажи]  where  [ДатаВремя]>=@StartDt and [ДатаВремя]<@StopDt  and [СуммаИтог]!=0 " +
" and [БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] in " +
"  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=821 and [cod_sgr]<=823)  )  and [НомерКлиента]!=6 and [НомерКлиента]!=54 " +
" and not [БарКод] in (" + CoffeeToGo + ")  group by [КодПодразд]";


            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("StartDt", StartDt);
            SqlParameter P2 = new SqlParameter("StopDt", StopDt);
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
                    Value2 = Convert.ToDouble(Sr.GetSqlDecimal(1).Value),
                };
                Tmp.Add(Mr);
            }
            Sr.Close();
            //Вино
            String Bs = GetBarCodesOfSalesGroup(3) + "," + GetBarCodesOfSalesGroup(20);
            q = "SELECT " +
"[КодПодразд], sum([СуммаИтог]) FROM [dbo].[Продажи]  where  [ДатаВремя]>=@StartDt and [ДатаВремя]<@StopDt  " +
" and [БарКод]  in   (Select [БаркодЧисл]  from [dbo].[barcod] where [cod_good] in " +
"  (SELECT [cod_good]  FROM [dbo].[katalog] where [cod_sgr]>=531 and [cod_sgr]<=532)  )  and [НомерКлиента]!=6 and [НомерКлиента]!=54 " +
"  group by [КодПодразд]";


            Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            P1 = new SqlParameter("StartDt", StartDt);
            P2 = new SqlParameter("StopDt", StopDt);
            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            Sr = Sc.ExecuteReader();
            //List<int> CoffCount = new List<int>();
            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                Tmp.FirstOrDefault(a => a.Department == Dep).Value3 = Convert.ToDouble(Sr.GetSqlDecimal(1).Value);
            }
            Sr.Close();



            //Все продажи
            q = "SELECT [КодПодразд] ,  sum([СуммаИтог])  FROM [dbo].[Продажи]  where [ДатаВремя]>=@StartDt and [ДатаВремя]<@StopDt and  [БарКод] not in  ( " + CoffeeToGo + " ) " +
 " and   [НомерКлиента]!=6 and [НомерКлиента]!=54 group by  [КодПодразд] ";


            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc2 = new SqlCommand(q, conn);
            P1 = new SqlParameter("StartDt", StartDt);
            P2 = new SqlParameter("StopDt", StopDt);
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
                    Tmp.FirstOrDefault(a => a.Department == Dep).Value = Convert.ToDouble(Sr2.GetSqlDecimal(1).Value);
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

