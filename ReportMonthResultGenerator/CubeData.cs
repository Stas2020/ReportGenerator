using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;



namespace ReportMonthResultGenerator
{
    class CubeData
    {

        public static string GetDishOfGroup(int GrNum)
        {
            string s = string.Format("{{sfsdf}}");
           
            string CoffeeToGo = "";

            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            conn1.Open();
            string q1 = "Select Number from [dbo].[Item] where [Id] in (SELECT [FK_ItemId]  FROM [dbo].[CategoryItem] where [FK_CategoryId] =( SELECT [Id] FROM [dbo].[Category] where Number ="+GrNum+"))";

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

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


        public static Dictionary<int,string > GetDishNames(String BarCodes)
        {
            List<ReportMonthResult> Tmp = new List<ReportMonthResult>();
            Dictionary<int,string > CoffeeToGo = new Dictionary<int,string> ();

            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            conn1.Open();
            string q1 = "Select Number, LongName from [dbo].[Item] where [Number] in (" + BarCodes + ")";

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc1 = new SqlCommand(q1, conn1);

            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                try
                {
                    CoffeeToGo.Add(Sr1.GetInt32(0), Sr1.GetString(1));
                }
                catch
                { 
                }
            }

          

            conn1.Close();

            return CoffeeToGo;

        }
        public static List<string> GetDishOfCat(int CatNum)
        {
            List<string> Tmp = new List<string>();
            string CoffeeToGo = "";

            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            //SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            SqlConnection conn1 = new SqlConnection(@"Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            conn1.Open();
            
            //string q1 = "Select Number from [dbo].[Item] where [Id] in (SELECT [FK_ItemId]  FROM [dbo].[CategoryItem] where [FK_CategoryId] =( SELECT [Id] FROM [dbo].[Category] where Number =" + CatNum + "))";
            string q1 =" SELECT * FROM[dbo].[GetDishezOfGeneralCat2] ( "+CatNum+")";

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc1 = new SqlCommand(q1, conn1);
            Sc1.CommandTimeout = 100000;
            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                Tmp.Add(Sr1.GetValue(0).ToString());
                //CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
            }

        //    CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);

            conn1.Close();

            return Tmp;

        }

        public static List<string> GetDishOfSalesCat(int CatNum)
        {
            List<string> Tmp = new List<string>();
            string CoffeeToGo = "";

            //BTestDataContext dBTest = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            //  SqlConnection conn1 = new SqlConnection(@"Data Source=AVRORA1\SQLEXPRESS;Initial Catalog=CFCInStoreDB;User ID=PDiscount;Password=PDiscount");
            SqlConnection conn1 = new SqlConnection(@"Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t; Connection Timeout=0");
            conn1.Open();
            
            //string q1 = "Select Number from [dbo].[Item] where [FK_SalesCategory] = ( SELECT [Id] FROM [dbo].[Category] where Number =" + CatNum + ")";
            string q1 = "SELECT [BarCode]  FROM[Diogen].[dbo].[AlohaMenuItemCatAll] where[CatId] = " + CatNum ;

            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc1 = new SqlCommand(q1, conn1);
            
            SqlDataReader Sr1 = Sc1.ExecuteReader();

            while (Sr1.Read())
            {
                Tmp.Add(Sr1.GetValue(0).ToString());
                //CoffeeToGo += (Sr1.GetValue(0).ToString()) + ",";
            }

        //    CoffeeToGo = CoffeeToGo.Substring(0, CoffeeToGo.Length - 1);

            conn1.Close();

            return Tmp;

        }
        public static string GetDishofSalesCatStr(int CatNum)
        {
           
            List<string> Tmp = GetDishOfSalesCat(CatNum);
            string res = "";
            foreach (string str in Tmp)
            {
                res += str + ",";
            }
            return res.Substring(0, res.Length - 1);


        }

        public static List<int> GetKitchenDList()
        {
            List<string> Pizza = GetDishOfSalesCat(33);
            Pizza.AddRange(GetDishOfSalesCat(34));
            Pizza.AddRange(GetDishOfSalesCat(35));
            //List<string> Tmp = GetDishOfCat(41).Except(Pizza).ToList();

            List<string> Tmp = GetDishOfCat(41);

            Tmp.AddRange(Pizza);


            return Tmp.Select(a=>Convert.ToInt32(a)).ToList();


        }

        public static string GetKitchenD()
         {
             List<string> Pizza = GetDishOfSalesCat(33);
             Pizza.AddRange(GetDishOfSalesCat(34));
               Pizza.AddRange(GetDishOfSalesCat(35));
            //List<string> Tmp = GetDishOfCat(41).Except(Pizza).ToList();

            List<string> Tmp = GetDishOfCat(41);
            List<string> Tmp2 = new List<string>();
            foreach (string p in Tmp)
            {
                if (!Pizza.Contains(p))
                {
                    Tmp2.Add(p);
                }
            }

            //Tmp.AddRange(Pizza);  

            var intercept = Tmp.Intersect(Pizza);
            string res = "";
            int i = intercept.Count();
            foreach (string str in Tmp2)
            {
                res += str + ",";
            }
            return res.Substring(0, res.Length - 1);


        }

        public static string GetStoikaBaristaD()
        {
            return GetDishOfGroup(49);

        }

        public static string GetNapitkiD()
        {


            return GetDishOfGroup(48) + "," + GetDishOfGroup(49) + "," + GetDishOfGroup(60);

        }
        public static string GetStoikaSpecD()
        {
            return string.Join(", ", GetDishOfCat(48)); // GetDishOfGroup(48);

        }

        public static string GetStoikaD()
        {
            //  return string.Join(", ", GetDishOfCat(48)) + string.Join(", ", GetDishOfCat(49));
            return string.Join(", ", GetDishOfCat(48))+ string.Join(", ", GetDishOfCat(49));
            //return GetDishOfGroup(48) + "," + GetDishOfGroup(49);

        }

        public static List<int> GetVineD()
        {
            List<string> V = GetDishOfSalesCat(20);
            V.AddRange(GetDishOfSalesCat(32));
            return V.Select(int.Parse).ToList();
        }

        public static string GetStoikaDWithoutCoffee()
        {
            List<string> Coffee = GetDishOfSalesCat(1);
            Coffee.AddRange(GetDishOfSalesCat(22));
            List<string> Stoyka = GetDishOfCat(49);
            Coffee.Sort();
            Stoyka.Sort();
            List<string> Tmp = Stoyka.Except(Coffee).ToList();
            string res = "";


            foreach (string str in Tmp)
            {
                res += str + ",";
            }

            res = res.Substring(0, res.Length - 1);

            //Dictionary<int, string> Dic = GetDishNames(res);
            
            Dictionary<int, string> Dic = GetDishNames(res);

            string Msg = "";
            foreach (int i in Dic.Keys)
            {
                Msg = i.ToString() + ". "+Dic[i]+Environment.NewLine;
                Console.WriteLine(Msg);
            }
            
            return res;




        }
        public static string GetBaristaD()
        {
            List<string> Coffee = GetDishOfSalesCat(1);
            Coffee.AddRange(GetDishOfSalesCat(22));
            List<string> Stoyka = GetDishOfCat(49);
            Coffee.Sort();
            Stoyka.Sort();
            List<string> Tmp = Stoyka.Except(Coffee).ToList();
            string res = "";


            foreach (string str in Tmp)
            {
                res += str + ",";
            }

            res = res.Substring(0, res.Length - 1);

            //Dictionary<int, string> Dic = GetDishNames(res);

            Dictionary<int, string> Dic = GetDishNames(res);

            string Msg = "";
            foreach (int i in Dic.Keys)
            {
                Msg = i.ToString() + ". " + Dic[i] + Environment.NewLine;
                Console.WriteLine(Msg);
            }

            return res;




        }

        public static Dictionary<int, double> GetSrCheck(DateTime Month)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            Dictionary<int, double> Tmp = new Dictionary<int, double>();
            Dictionary<int, int> ChkCounts = new Dictionary<int, int>();

            string q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and [НомерСтроки]=1 " +
      
    " group by  [КодПодразд] ";
                       
            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();
            
            while (Sr2.Read())
            {
                try
                {
                    ChkCounts.Add(Sr2.GetInt32(0),Sr2.GetInt32(1));
                }
                catch
                { }
            }
            Sr2.Close();

            q = "SELECT [КодПодразд] ,  sum([СуммаИтог])  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 " +

    " group by  [КодПодразд] ";

            SqlCommand Sc3 = new SqlCommand(q, conn);
            P1 = new SqlParameter("year", Month.Year);
            P2 = new SqlParameter("Month", Month.Month);
            Sc3.Parameters.Add(P1);
            Sc3.Parameters.Add(P2);
            Sc3.CommandTimeout = 0;
            SqlDataReader Sr3 = Sc3.ExecuteReader();
            
            while (Sr3.Read())
            {
                try
                {
                    int Depnum = Sr3.GetInt32(0);
                    int mChkCount = 0;
                    ChkCounts.TryGetValue(Depnum, out mChkCount);
                    if (mChkCount > 0)
                    {
                        double MoneyCount = (double)Sr3.GetDecimal(1);
                        Tmp.Add(Depnum, MoneyCount / (double)mChkCount);
                    }
                    else
                    {
                        Tmp.Add(Depnum, 0);
                    
                    }
                }
                catch
                { }
            }
            Sr2.Close();

            return Tmp;
        }

        public static Dictionary<int, decimal> GetMoneyCount(DateTime Month)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            //  Dictionary<int, double> Tmp = new Dictionary<int, double>();
            var ChkCounts = new Dictionary<int, decimal>();

            string q = "SELECT [КодПодразд] ,  Sum([СуммаИтог])  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month " +

    " group by  [КодПодразд] ";

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();

            while (Sr2.Read())
            {
                try
                {
                    ChkCounts.Add(Sr2.GetInt32(0), (decimal)Sr2.GetDecimal(1));
                }
                catch
                { }
            }
            Sr2.Close();



            return ChkCounts;
        }


        public static Dictionary<int, decimal> GetMoneyCountByDay(DateTime Day)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            //  Dictionary<int, double> Tmp = new Dictionary<int, double>();
            var ChkCounts = new Dictionary<int, decimal>();

            string q = "SELECT [КодПодразд] ,  Sum([СуммаИтог])  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month and [Число]=@Day " +

    " group by  [КодПодразд] ";

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("year", Day.Year);
            SqlParameter P2 = new SqlParameter("Month", Day.Month);
            SqlParameter P3 = new SqlParameter("Day", Day.Day);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.Parameters.Add(P3);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();

            while (Sr2.Read())
            {
                try
                {
                    ChkCounts.Add(Sr2.GetInt32(0), (decimal)Sr2.GetDecimal(1));
                }
                catch
                { }
            }
            Sr2.Close();



            return ChkCounts;
        }



        public static Dictionary<int, int> GetChecksCountWithoutDeleveryByDay(DateTime day)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            //  Dictionary<int, double> Tmp = new Dictionary<int, double>();
            Dictionary<int, int> ChkCounts = new Dictionary<int, int>();

            string q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month and [Число]=@day and [СуммаИтог]!=0 and [НомерСтроки]=1 and not ([Стол]>=200 and [Стол]<=254)" +

    " group by  [КодПодразд] ";

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("year", day.Year);
            SqlParameter P2 = new SqlParameter("Month", day.Month);
            SqlParameter P3 = new SqlParameter("day", day.Day);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.Parameters.Add(P3);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();

            while (Sr2.Read())
            {
                try
                {


                    ChkCounts.Add(Sr2.GetInt32(0), Sr2.GetInt32(1));
                }
                catch
                { }
            }
            Sr2.Close();

            /*
            if (ChkCounts.Keys.Any(a => a == 191) && ChkCounts.Keys.Any(a => a == 190))
            {
                ChkCounts[190] += ChkCounts[191];
            }
            */
            return ChkCounts;
        }


        public static Dictionary<int, int> GetChecksCountWithoutDelevery(DateTime Month)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

            //  Dictionary<int, double> Tmp = new Dictionary<int, double>();
            Dictionary<int, int> ChkCounts = new Dictionary<int, int>();

            string q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and [НомерСтроки]=1 and not ([Стол]>=200 and [Стол]<=254)" +

    " group by  [КодПодразд] ";

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();

            while (Sr2.Read())
            {
                try
                {
                    

                    ChkCounts.Add(Sr2.GetInt32(0), Sr2.GetInt32(1));
                }
                catch
                { }
            }
            Sr2.Close();
            /*
            if (ChkCounts.Keys.Any(a => a == 191) && ChkCounts.Keys.Any(a => a == 190))
            {
                ChkCounts[190] += ChkCounts[191];
            }
            */

            return ChkCounts;
        }




        public static Dictionary<int, int> GetChecksCount(DateTime Month)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            conn.Open();

          //  Dictionary<int, double> Tmp = new Dictionary<int, double>();
            Dictionary<int, int> ChkCounts = new Dictionary<int, int>();

            string q = "SELECT [КодПодразд] ,  count(*)  FROM [dbo].[Vall]  where [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and [НомерСтроки]=1 " +

    " group by  [КодПодразд] ";

            SqlCommand Sc2 = new SqlCommand(q, conn);
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);
            Sc2.Parameters.Add(P1);
            Sc2.Parameters.Add(P2);
            Sc2.CommandTimeout = 0;
            SqlDataReader Sr2 = Sc2.ExecuteReader();

            while (Sr2.Read())
            {
                try
                {
                    ChkCounts.Add(Sr2.GetInt32(0), Sr2.GetInt32(1));
                }
                catch
                { }
            }
            Sr2.Close();

          

            return ChkCounts;
        }



        public static Dictionary<int, int> GetAllChkCountByDay(DateTime Day)
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

        public static string  GetDishesBkofAllLockal(string Bk)
        {
            string Tmp = "";

            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();

            string q = "SELECT distinct [bar_cod] from dbo.barcod where [cod_good] in " +
            " (	select [cod_good] from dbo.barcod where [bar_cod] in (" + Bk + ") ) ";

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;

            SqlDataReader Sr = Sc.ExecuteReader();

            while (Sr.Read())
            {
                Tmp += Sr.GetString(0) + ",";
                
               
            }
            Sr.Close();
            Tmp = Tmp.Substring(0, Tmp.Length - 1);
            return Tmp;
        }

        public static List<DishCount> GetDishesCountNoDelevery(DateTime Month, string BarCodes, bool AllCodes=false, int Day = 0)
        {
            List<DishCount> Tmp = new List<DishCount>();

            // SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]),sum([СуммаИтогРуб])  FROM [dbo].[Vall] a where  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and not ([Стол]>=200 and [Стол]<=254) ";



            //"[КодПодразд], sum([Колич]),sum([СуммаИтогРуб])  FROM [dbo].[Vall]  where  [Год]=@year and [Месяц]=@Month ";
            if (Day != 0)
            {
                q += $" and [Число]= {Day} ";
            }


            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]<=20 and [Колич]>-20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();

            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                DishCount Mr = new DishCount()
                {

                    Dep = Dep,

                    Count = (int)Sr.GetDecimal(1),

                    MoneyCount = (decimal)Sr.GetDecimal(2)
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            q = "SELECT " +
"[КодПодразд], Count(*),sum([СуммаИтогРуб]) FROM [dbo].[Vall]  where  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 and not ([Стол]>=200 and [Стол]<=254) ";

            if (Day != 0)
            {
                q += $" and [Число]= {Day} ";
            }
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]>20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            P1 = new SqlParameter("year", Month.Year);
            P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            Sr = Sc.ExecuteReader();



            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);


                Tmp.FirstOrDefault(a => a.Dep == Dep).Count += Sr.GetInt32(1);
                Tmp.FirstOrDefault(a => a.Dep == Dep).MoneyCount += Sr.GetDecimal(2);


                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            conn.Close();
            return Tmp;

        }


        public static List<DishCount> GetDishesCount(DateTime Month,string BarCodes,bool AllCodes,int Day=0)
        {
            List<DishCount> Tmp = new List<DishCount>();

           // SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]),sum([СуммаИтогРуб])  FROM [dbo].[Vall] a where  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 ";



//"[КодПодразд], sum([Колич]),sum([СуммаИтогРуб])  FROM [dbo].[Vall]  where  [Год]=@year and [Месяц]=@Month ";
            if (Day != 0)
            {
                q += $" and [Число]= {Day} ";
            }


            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q+=" and [Колич]<=20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

                SqlDataReader Sr = Sc.ExecuteReader();

            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                DishCount Mr = new DishCount()
                {

                    Dep = Dep,
                    
                    Count = (int)Sr.GetDecimal(1),
                    
                    MoneyCount = (decimal)Sr.GetDecimal(2)
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            q = "SELECT " +
"[КодПодразд], Count(*),sum([СуммаИтогРуб]) FROM [dbo].[Vall]  where  [Год]=@year and [Месяц]=@Month and [СуммаИтог]!=0 ";
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q+=" and [Колич]>20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            P1 = new SqlParameter("year", Month.Year);
            P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            Sr = Sc.ExecuteReader();



            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);


                Tmp.FirstOrDefault(a => a.Dep == Dep).Count += Sr.GetInt32(1);
                Tmp.FirstOrDefault(a => a.Dep == Dep).MoneyCount += Sr.GetDecimal(2);


                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            conn.Close();
            return Tmp;

        }

        public static List<DishCount> GetDishesCountYear(DateTime Month, string BarCodes, bool AllCodes)
        {
            List<DishCount> Tmp = new List<DishCount>();

            // SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");
            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]),sum([СуммаИтогРуб])  FROM [dbo].[Vall]  where  [Год]=@year and [СуммаИтог]!=0 ";
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]<=20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            

            Sc.Parameters.Add(P1);
            

            SqlDataReader Sr = Sc.ExecuteReader();

            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                DishCount Mr = new DishCount()
                {

                    Dep = Dep,

                    Count = Sr.GetDecimal(1),

                    MoneyCount = Sr.GetDecimal(2)
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            q = "SELECT " +
"[КодПодразд], Count(*),sum([СуммаИтогРуб]) FROM [dbo].[Vall]  where  [Год]=@year and [СуммаИтог]!=0 ";
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]>20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            P1 = new SqlParameter("year", Month.Year);

            Sc.Parameters.Add(P1);

            Sr = Sc.ExecuteReader();



            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);


                Tmp.FirstOrDefault(a => a.Dep == Dep).Count += Sr.GetInt32(1);
                Tmp.FirstOrDefault(a => a.Dep == Dep).MoneyCount += Sr.GetDecimal(2);


                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            conn.Close();
            return Tmp;

        }

        public static List<DishCount> GetDishesCountByDays(DateTime Month, string BarCodes, bool AllCodes)
        {
            List<DishCount> Tmp = new List<DishCount>();

            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], sum([Колич]),sum([СуммаИтогРуб])  FROM [dbo].[Vall]  where ( [Год]=@year and [Месяц]=@Month and  ([Число]=24))and [СуммаИтог]!=0 ";
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]<=20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();

            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                DishCount Mr = new DishCount()
                {

                    Dep = Dep,

                    Count = Sr.GetInt32(1),

                    MoneyCount = Sr.GetDecimal(2)
                };
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            q = "SELECT " +
"[КодПодразд], Count(*),sum([СуммаИтогРуб]) FROM [dbo].[Vall]  where  ( [Год]=@year and [Месяц]=@Month and  [Число]=24 ) and [СуммаИтог]!=0 ";
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]>20 group by [КодПодразд]";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            P1 = new SqlParameter("year", Month.Year);
            P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            Sr = Sc.ExecuteReader();



            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);


                Tmp.FirstOrDefault(a => a.Dep == Dep).Count += Sr.GetInt32(1);
                Tmp.FirstOrDefault(a => a.Dep == Dep).MoneyCount += Sr.GetDecimal(2);


                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

            conn.Close();
            return Tmp;

        }
        //public static List<DishCount> GetDishesCountbyTime(DateTime Dt1,DateTime Dt2, List<int> BarCodes, bool AllCodes)
        public static List<DishCount> GetDishesCountbyTime(DateTime Month, string BarCodes, bool AllCodes)
        {
            List<DishCount> Tmp = new List<DishCount>();
            /*
            BTestDataContext Dc = new BTestDataContext(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            Dc.CommandTimeout = 1000000;

            var res = (from o in Dc.Продажиs where (o.ДатаВремя > Dt1 && o.ДатаВремя < Dt2 && BarCodes.Contains(o.БарКод.Value) && o.СуммаИтог > 0 && o.Колич < 20 && o.КодПодразд==260) select o);

            foreach (Продажи s in res)
            {
                DishCount DC = new DishCount();
                DC.Dep = s.КодПодразд.Value;
                DC.Count = s.Колич.Value;
                DC.dt = s.ДатаВремя.Value;
                DC.MoneyCount = s.СуммаИтог.Value;

                Tmp.Add(DC);
            }

            return Tmp;
            */

            List<object> n = new List<object>();
            try
            { }
            catch(Exception)
            {
                throw;
            }

            SqlConnection conn = new SqlConnection(@"Data Source=192.168.254.172;Initial Catalog=Btest;User ID=manager;Password=manager");

            conn.Open();
            string q = "SELECT " +
"[КодПодразд], [Колич], [СуммаИтогРуб] ,[ДатаВремя] ,[Час] FROM [dbo].[Продажи]  where  [Год]=@year and [Месяц]=@Month and [СуммаИтог]>0 ";
            if (!AllCodes)
            {
                q += " and [БарКод]  in    (" + BarCodes + ") ";
            }
            q += " and [Колич]<=20 ";
            //IQueryable <Продажи> CoffeeCount = from o in dBTest where 

            SqlCommand Sc = new SqlCommand(q, conn);
            Sc.CommandTimeout = 0;
            SqlParameter P1 = new SqlParameter("year", Month.Year);
            SqlParameter P2 = new SqlParameter("Month", Month.Month);

            Sc.Parameters.Add(P1);
            Sc.Parameters.Add(P2);

            SqlDataReader Sr = Sc.ExecuteReader();

            while (Sr.Read())
            {
                int Dep = Sr.GetInt32(0);
                DishCount Mr = new DishCount()
                {

                    Dep = Dep,

                    Count = Sr.GetInt32(1),

                    MoneyCount = Sr.GetDecimal(2),
                    //dt  = Sr.GetDateTime(3)
                };
                DateTime Dt = Sr.GetDateTime(3);
                int h = Sr.GetInt32(4);
                Mr.dt = new DateTime(Dt.Year, Dt.Month, Dt.Day, h, 10, 0);
                Tmp.Add(Mr);
                //    CoffCount.Add(Sr.GetInt32(0));
            }
            Sr.Close();

         

            conn.Close();
            return Tmp;
           
        }

        public static List<DishCount> GetAllDishesCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
         //   string Kd = GetKitchenD();
            return GetDishesCount(Month, "", true );
        }
        public static List<DishCount>  GetKitchenDishesCount(DateTime Month,bool Year=false,int Day=0)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetKitchenD();
            if (Year)
            {
              return  GetDishesCountYear(Month, Kd, false);
            }
            else
            {
                return GetDishesCount(Month, Kd, false, Day);
            }
        }

        public static List<DishCount> GetKitchenDishesCount2(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetKitchenD();
            return GetDishesCountByDays(Month, Kd, false);
        }

        public static List<DishCount> GetBaristaDishesCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetStoikaBaristaD();
            return GetDishesCount(Month, Kd, false);
        }
        public static List<DishCount> GetBaristaDishesCountWithTime(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetStoikaBaristaD();
            List<int> D = Kd.Split(","[0]).Select(x => Convert.ToInt32(x)).ToList();

            //return GetDishesCountbyTime(Month, Month.AddMonths(1), D, false);
            return GetDishesCountbyTime(Month, Kd, false);
        }
        /*
        public static List<DishCount> GetStoykaDishesCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetStoikaD();
            return GetDishesCount(Month, Kd, false);
        }
*/
        public static List<DishCount> GetNapitkiDishesCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetNapitkiD();
            return GetDishesCount(Month, Kd, false);
        }

        public static List<DishCount> GetGorCexCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetDishOfGroup(58);
            return GetDishesCount(Month, Kd, false);
        }
        public static List<DishCount> GetHolCexCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetDishOfGroup(57);
            Dictionary<int, string> Dic = GetDishNames(Kd);

            string Msg = "";
            foreach (int i in Dic.Keys)
            {
                Msg = i.ToString() + ". " + Dic[i] + Environment.NewLine;
                Console.WriteLine(Msg);
            }

            return GetDishesCount(Month, Kd, false);
        }
        public static List<DishCount> GetSpecDishesCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetStoikaSpecD();

          //  Dictionary<int, string> Dic = GetDishNames(Kd);
            /*
            string Msg = "";
            foreach (int i in Dic.Keys)
            {
                Msg = i.ToString() + ". " + Dic[i] + Environment.NewLine;
                Console.WriteLine(Msg);
            }
            */
            return GetDishesCount(Month, Kd, false);
        }
        public static List<DishCount> GetStoikaDishesCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetStoikaD();
            return GetDishesCount(Month, Kd,false);
        }
        public static List<DishCount> GetStoikaDesertCount(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetDishOfGroup(54);
            //Kd = Kd.Substring
            return GetDishesCount(Month, Kd, false);
        }
        public static List<DishCount> GetStoikaDishesCountWithoutCoffee(DateTime Month)
        {
            List<DishCount> Tmp = new List<DishCount>();
            string Kd = GetStoikaDWithoutCoffee();
            return GetDishesCount(Month, Kd, false);
        }
        
    }

    public class DishCount
    {
        public int Dep = 0;
        public decimal Count = 0;
        public decimal MoneyCount = 0;
        public DateTime dt = new DateTime();
    }
}
