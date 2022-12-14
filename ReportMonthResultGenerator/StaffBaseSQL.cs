using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.Data.SqlClient;

namespace ReportMonthResultGenerator
{
 
    class  StaffBaseSQL
    {
        static string c2Sql = @"Data Source=s2010;Initial Catalog=Diogen;User ID=quasiadm;Password=Fil123fil123";
        //static string c2 = "Driver={Progress OpenEdge 10.2B Driver};HOST=web;DB=staff;UID=sysprogress;PWD=progress;PORT=2520;";
        //static string c2 = "Driver={Progress OpenEdge 10.1B driver};HOST=web;DB=staff;UID=sysprogress;PWD=progress;PORT=2520;"; //Было так
        static string c2Dev1 = "Driver={Progress OpenEdge 10.1B driver};HOST=develop1;DB=sal_staff;UID=sysprogress;PWD=progress;PORT=2520;";

        public static List<CEmpl> getPeopleOfPosOld(string  Poss, DateTime Month)
        {
            /*
            string CommandStr = "   SELECT        a.EMPLOYEE_ID " +
       "FROM            PUB.EMPLOYEE_HISTORY a " +
       "WHERE        (UPDATE_DATE < '1/11/2013') AND (POSITION_ID = " + Poss + ") AND (UPDATE_DATE = " +
                                   " (SELECT        MAX(UPDATE_DATE) AS Expr1 " +
                                     " FROM            PUB.EMPLOYEE_HISTORY b " +
                                     " WHERE        (a.EMPLOYEE_ID = b.EMPLOYEE_ID) AND (UPDATE_DATE < '1/11/2013') and  (POSITION_ID  = " + Poss + "))) ";


            */


            /*string CommandStr = " SELECT DISTINCT EMPLOYEE_ID FROM            PUB.EMPLOYEE_HISTORY a WHERE        (UPDATE_DATE < '"+Month.ToString("dd'/'MM'/'yyyy") +"') AND (UPDATE_DATE = (SELECT        MAX(b.UPDATE_DATE) AS Expr1 " +
                               " FROM            PUB.EMPLOYEE_HISTORY b WHERE        (a.EMPLOYEE_ID = b.EMPLOYEE_ID) AND (b.UPDATE_DATE < '" + Month.ToString("dd'/'MM'/'yyyy") + "')  GROUP BY b.EMPLOYEE_ID)) AND (a.POSITION_ID = "+Poss+") AND (a.EMPLOYEE_ID NOT IN (SELECT  EMPLOYEE_ID " +
                              " FROM            PUB.EMPLOYEE     WHERE        (DISMISSAL_DATE < '" + Month.ToString("dd'/'MM'/'yyyy") + "'))) ";*/
            string CommandStr = " SELECT DISTINCT EMPLOYEE_ID FROM            staff.staff.PUB.EMPLOYEE_HISTORY a WHERE        (UPDATE_DATE < '" + Month.ToString("dd'/'MM'/'yyyy") + "') AND (UPDATE_DATE = (SELECT        MAX(b.UPDATE_DATE) AS Expr1 " +
                               " FROM            staff.staff.PUB.EMPLOYEE_HISTORY b WHERE        (a.EMPLOYEE_ID = b.EMPLOYEE_ID) AND (b.UPDATE_DATE < '" + Month.ToString("dd'/'MM'/'yyyy") + "')  GROUP BY b.EMPLOYEE_ID)) AND (a.POSITION_ID = " + Poss + ") AND (a.EMPLOYEE_ID NOT IN (SELECT  EMPLOYEE_ID " +
                              " FROM            staff.staff.PUB.EMPLOYEE     WHERE        (DISMISSAL_DATE < '" + Month.ToString("dd'/'MM'/'yyyy") + "'))) ";




            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<CEmpl> empls = new List<CEmpl>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn); //OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            //OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);

            //Comm.Parameters.Add(new OdbcParameter("p1", StartDt));
            //Comm.Parameters.Add(new OdbcParameter("p2", EndDt));
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader(); //OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmpl Emp = new CEmpl()
                        {
                            Id = OdR.GetInt32(0),
                            Pos = Convert.ToInt32(Poss),                     
                        };

                        empls.Add(Emp);
}
                    catch
                    { }
                }
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.Message);
            }
            return empls;


        }


        public static DateTime GetEntry_date(int EmpNum)
        {
            //int SuShefPosNum = 8;

            /*string CommandStr = "SELECT        PUB.EMPLOYEE.ENTRY_DATE " +
"FROM            PUB.EMPLOYEE " +
"WHERE      PUB.EMPLOYEE.EMPLOYEE_ID  =" + EmpNum.ToString();*/
            string CommandStr = "SELECT        EMPLOYEE.ENTRY_DATE " +
"FROM            staff.staff.PUB.EMPLOYEE " +
"WHERE      EMPLOYEE.EMPLOYEE_ID  =" + EmpNum.ToString();


            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<CEmpl> empls = new List<CEmpl>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn);// OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            //OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);

            
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader(); //OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {

                    return OdR.GetDateTime(0);

                    
                }
                OdR.Close();
            }
            catch
            { 
            
            }
            Conn.Close();
            return new DateTime (1900,1,1);
            
         
        }



        public static List<CEmpl> GetAllEmpl(DateTime StartDt, DateTime EndDt)
        {
            //int SuShefPosNum = 8;

            /*string CommandStr = "SELECT        PUB.EMPLOYEE.EMPLOYEE_ID, PUB.EMPLOYEE.LAST_NAME, PUB.EMPLOYEE.FIRST_NAME, PUB.EMPLOYEE.MIDDLE_NAME,PUB.EMPLOYEE.POSITION_ID " +
"FROM            PUB.EMPLOYEE " +
"WHERE        ";
            CommandStr += " (dismissal_date is null  ";
            CommandStr += "or  (dismissal_date>? and dismissal_date<? ))";*/

            string CommandStr = "SELECT        EMPLOYEE.EMPLOYEE_ID, EMPLOYEE.LAST_NAME, EMPLOYEE.FIRST_NAME, EMPLOYEE.MIDDLE_NAME,EMPLOYEE.POSITION_ID " +
"FROM            staff.staff.PUB.EMPLOYEE " +
"WHERE        ";
            CommandStr += " (dismissal_date is null  ";
            CommandStr += "or  (dismissal_date>@p1 and dismissal_date<@p2 ))";

            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<CEmpl> empls = new List<CEmpl>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn); //OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            //OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);

            Comm.Parameters.Add(new SqlParameter("p1", StartDt));//Comm.Parameters.Add(new OdbcParameter("p1", StartDt));
            Comm.Parameters.Add(new SqlParameter("p2", EndDt));//Comm.Parameters.Add(new OdbcParameter("p2", EndDt));
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader(); //OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmpl Emp = new CEmpl()
                        {
                            Id = OdR.GetInt32(0),
                            Name = OdR.GetString(1) + " " + OdR.GetString(2),
                            Pos = OdR.GetInt32(4),

                        };


                        empls.Add(Emp);

                    }
                    catch
                    { }
                }
            }
            catch
            { }
            return empls;

        }


        public static List<CEmpl> GetSuShefs(DateTime StartDt, DateTime EndDt)
        {
            int SuShefPosNum = 8;

            /*string CommandStr = "SELECT        PUB.EMPLOYEE.EMPLOYEE_ID, PUB.EMPLOYEE.LAST_NAME, PUB.EMPLOYEE.FIRST_NAME, PUB.EMPLOYEE.MIDDLE_NAME " +
"FROM            PUB.EMPLOYEE " +
"WHERE        PUB.EMPLOYEE.POSITION_ID = " + SuShefPosNum.ToString();
            CommandStr += "and (dismissal_date is null  ";
            CommandStr += "or  (dismissal_date>? and dismissal_date<? ))";*/
            string CommandStr = "SELECT        EMPLOYEE.EMPLOYEE_ID, EMPLOYEE.LAST_NAME, EMPLOYEE.FIRST_NAME, EMPLOYEE.MIDDLE_NAME " +
"FROM            staff.staff.PUB.EMPLOYEE " +
"WHERE        EMPLOYEE.POSITION_ID = " + SuShefPosNum.ToString();
            CommandStr += "and (dismissal_date is null  ";
            CommandStr += "or  (dismissal_date>p1 and dismissal_date<p2 ))";

            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<CEmpl> empls = new List<CEmpl>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn); //OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            //OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);

            Comm.Parameters.Add(new SqlParameter("p1", StartDt)); //Comm.Parameters.Add(new OdbcParameter("p1", StartDt));
            Comm.Parameters.Add(new SqlParameter("p2", EndDt)); //Comm.Parameters.Add(new OdbcParameter("p2", EndDt));
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader(); //OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmpl Emp = new CEmpl()
                        {
                            Id = OdR.GetInt32(0),
                            Name = OdR.GetString(1) + " " + OdR.GetString(2),
                            
                            
                        };


                        empls.Add(Emp);

                    }
                    catch
                    { }
                }
            }
            catch
            { }
            return empls;
        
        }





        public static List<CEmplWt> GetWtsByDep(int dep, DateTime StartDt, DateTime EndDt)
        {

            /*string CommandStr = "SELECT SUBDIVISION_ID, dtt_arrival, dtt_departure, EMPLOYEE_ID,POSITION_ID " +
"FROM            PUB.WORKING_TIME " +
"WHERE        ( ";
            CommandStr += $" dtt_departure>? and dtt_arrival<? and SUBDIVISION_ID={dep})";*/
            string CommandStr = "SELECT SUBDIVISION_ID, dtt_arrival, dtt_departure, EMPLOYEE_ID,POSITION_ID " +
"FROM            staff.staff.PUB.WORKING_TIME " +
"WHERE        ( ";
            CommandStr += $" dtt_departure>p1 and dtt_arrival<p2 and SUBDIVISION_ID={dep})";


            SqlConnection Conn = new SqlConnection(c2Sql); // OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<CEmplWt> emplsWt = new List<CEmplWt>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn);//OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            Comm.Parameters.Add(new SqlParameter("p1", StartDt)); //Comm.Parameters.Add(new OdbcParameter("p1", StartDt));
            Comm.Parameters.Add(new SqlParameter("p2", EndDt)); //Comm.Parameters.Add(new OdbcParameter("p2", EndDt));
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader(); // OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmplWt EmpWt = new CEmplWt()
                        {
                            Dep = OdR.GetInt32(0),
                            StartDt = StaffWtToExcel.GetMaxDate(StartDt, OdR.GetDateTime(1)),
                            StopDt = StaffWtToExcel.GetMinDate(EndDt, OdR.GetDateTime(2)),


                        };
                        try
                        {
                            EmpWt.Emp = new CEmpl()
                            {
                                Pos = OdR.GetInt32(4)
                            };
                        }
                        catch
                        {
                        }

                        if ((EmpWt.StopDt - EmpWt.StartDt).TotalDays < 0.8)
                        {
                            emplsWt.Add(EmpWt);
                        }

                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Conn.Close();
            return emplsWt;

        }


        public static List<CEmplWt> GetWts(List<CEmpl> Empls, DateTime StartDt, DateTime EndDt)
        {


            string EmplsStr = "";
            if ((Empls==null)||(Empls.Count == 0))
            {

            }
            else
            {

                foreach (CEmpl Empl in Empls)
                {
                    EmplsStr += Empl.Id + ",";

                }

                EmplsStr = EmplsStr.Substring(0, EmplsStr.Length - 1);
            }

            /*string CommandStr = "SELECT        SUBDIVISION_ID, dtt_arrival, dtt_departure, EMPLOYEE_ID " + 
"FROM            PUB.WORKING_TIME " + 
"WHERE        ( " ;
            CommandStr += " dtt_departure>? and dtt_arrival<? ";*/

            string CommandStr = "SELECT        SUBDIVISION_ID, dtt_arrival, dtt_departure, EMPLOYEE_ID " +
"FROM            staff.staff.PUB.WORKING_TIME " +
"WHERE        ( ";
            CommandStr += " dtt_departure>p1 and dtt_arrival<p2 ";

            if (EmplsStr.Length > 0)
            {
                CommandStr += " and EMPLOYEE_ID in ( " + EmplsStr + ") ";
            }
            CommandStr += " )";

            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<CEmplWt> emplsWt = new List<CEmplWt>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn); //OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            Comm.Parameters.Add(new SqlParameter("p1", StartDt));// Comm.Parameters.Add(new OdbcParameter("p1", StartDt));
            Comm.Parameters.Add(new SqlParameter("p2", EndDt)); //Comm.Parameters.Add(new OdbcParameter("p2", EndDt));
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader(); //OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmplWt EmpWt = new CEmplWt()
                        {
                            Dep = OdR.GetInt32(0),
                            StartDt = StaffWtToExcel.GetMaxDate(StartDt, OdR.GetDateTime(1)),
                            StopDt = StaffWtToExcel.GetMinDate(EndDt, OdR.GetDateTime(2)),
                        

                        };
                        try
                        {
                            EmpWt.Emp = Empls.Where(a => a.Id == OdR.GetInt32(3)).First();
                        }
                        catch
                        {
                        }
                        
                        if ((EmpWt.StopDt - EmpWt.StartDt).TotalDays < 0.8)
                        {
                            emplsWt.Add(EmpWt);
                        }

                    }
                    catch(Exception ee)
                    {
                        Console.WriteLine(ee.Message);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Conn.Close();
            return emplsWt;

        }


        public static List<CEmpl> GetEmplsOfPos(DateTime StartDt, List<int> Pos)
        {
            
            if ( Pos.Count==0) return null;
            string Poss = "";
            foreach (int p in Pos)
            {
                Poss += p.ToString() + ",";
            }
            Poss = Poss.Substring(0, Poss.Length - 1);


            /*           string CommandStr = "SELECT        PUB.EMPLOYEE.EMPLOYEE_ID, PUB.EMPLOYEE.LAST_NAME, PUB.EMPLOYEE.FIRST_NAME, PUB.EMPLOYEE.MIDDLE_NAME, PUB.EMPLOYEE.POSITION_ID " +
           "FROM            PUB.EMPLOYEE " +
           "WHERE        PUB.EMPLOYEE.POSITION_ID in (" + Poss+")";*/
            string CommandStr = "SELECT        EMPLOYEE.EMPLOYEE_ID, EMPLOYEE.LAST_NAME, EMPLOYEE.FIRST_NAME, EMPLOYEE.MIDDLE_NAME, EMPLOYEE.POSITION_ID " +
"FROM            staff.staff.PUB.EMPLOYEE " +
"WHERE        EMPLOYEE.POSITION_ID in (" + Poss + ")";
            CommandStr += " and (dismissal_date is null  ";
            CommandStr += " or  (dismissal_date>@p1 ))";//CommandStr += "or  (dismissal_date>? ))";

            SqlConnection Conn = new SqlConnection(c2Sql); //OdbcConnection Conn = new OdbcConnection(c2);            
            //var sdfsdfsfd = Conn.Driver;
            Conn.Open();

            List<CEmpl> empls = new List<CEmpl>();

            SqlCommand Comm = new SqlCommand(CommandStr, Conn);//OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            //OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);

            Comm.Parameters.Add(new SqlParameter("p1", StartDt));//Comm.Parameters.Add(new OdbcParameter("p1", StartDt));
            //Comm.Parameters.Add(new OdbcParameter("p2", EndDt));
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader();//OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmpl Emp = new CEmpl()
                        {
                            Id = OdR.GetInt32(0),
                            Name = OdR.GetString(1) + " " + OdR.GetString(2),
                            Pos = OdR.GetInt32(4),

                        };

                        if ((Emp.Id==10799)||(Emp.Id==8259)||(Emp.Id==5429)) continue;

                        empls.Add(Emp);

                    }
                    catch(Exception ex)
                    {
                        ;
                    }
                }
            }
            catch(Exception e)
            {

                Console.WriteLine(e.Message);
            }
            return empls;

        }


        public static List<CEmpl> GetAllSal(DateTime StartDt, List<int> Empls)
        {

            List<CEmpl> Tmp3 = new List<CEmpl>();


            string empls = "";
            if (Empls.Count == 0) return null;

            foreach (int p in Empls)
            {
                empls += p.ToString() + ",";
            }
            empls = empls.Substring(0, empls.Length - 1);

            

            /*string CommandStr = "SELECT        EMPLOYEE_ID, sal, sal_date, subdivision_id " +
            "FROM  PUB.rep_sal_hist where EMPLOYEE_ID in (" + empls + ") and sal_date =? ";*/
            string CommandStr = "SELECT        EMPLOYEE_ID, sal, sal_date, subdivision_id " +
            "FROM  staff.staff.PUB.rep_sal_hist where EMPLOYEE_ID in (" + empls + ") and sal_date =p1 ";

            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();


            SqlCommand Comm = new SqlCommand(CommandStr, Conn);//OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            SqlParameter p1 = new SqlParameter("p1", OdbcType.DateTime);//OdbcParameter p1 = new OdbcParameter("p1", OdbcType.DateTime);
            p1.Value = StartDt;
            Comm.Parameters.Add(p1);
            List<SalaryTable> SalT = new List<SalaryTable>();
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader();// OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        CEmpl emp = new CEmpl ()
                        {
                            Id = OdR.GetInt32(0),
                            Money = Convert.ToDouble(OdR.GetValue(1).ToString().Replace(".",",")),
                        
                        };
                        Tmp3.Add(emp);
                    }
                    catch
                    { }
                }
            }
            catch
            { }
            Comm.Dispose();
            Conn.Close();

            
            return Tmp3;
        }



        public static List<CEmpl> GetSal(DateTime StartDt,int EmpId)
        {

            List<CEmpl> Tmp3 = new List<CEmpl>();




            /*string CommandStr ="SELECT        EMPLOYEE_ID, sal, sal_date, subdivision_id "+
                "FROM            PUB.rep_sal_hist where   EMPLOYEE_ID =" + EmpId.ToString();
//"FROM            PUB.rep_sal_hist where sal_date<? and EMPLOYEE_ID ="+EmpId.ToString();*/
            string CommandStr = "SELECT        EMPLOYEE_ID, sal, sal_date, subdivision_id " +
                "FROM            staff.staff.PUB.rep_sal_hist where   EMPLOYEE_ID =" + EmpId.ToString();

            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();


            SqlCommand Comm = new SqlCommand(CommandStr, Conn);//OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            SqlParameter p1 = new SqlParameter("p1",OdbcType.DateTime);//OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);
            p1.Value = StartDt;
            Comm.Parameters.Add(p1);
            List<SalaryTable> SalT = new List<SalaryTable>();
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader();//OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        SalaryTable st = new SalaryTable()
                        {
                            dtChange = OdR.GetDateTime(2),
                            EmpId = OdR.GetInt32(0),
                            sal = Convert.ToDouble(OdR.GetValue(1).ToString().Replace("."[0], ","[0]))

                        };
                        SalT.Add(st);
                    }
                    catch
                    { }
                }
            }
            catch
            { }
            Comm.Dispose();
            Conn.Close();

            Tmp3 = (from o in SalT

                    group o by o.EmpId into vals
                    let MaxDt = vals.Max(a => a.dtChange)
                    select new CEmpl()
                    {
                        Id = vals.FirstOrDefault(a => a.dtChange == MaxDt).EmpId,
                        sal = vals.FirstOrDefault(a => a.dtChange == MaxDt).sal


                    }
                 ).ToList();

            return Tmp3;
        }

        public static List<StaffParams> GetParametrsOfStaff(DateTime StartDt)
        {


            /*string CommandStr ="SELECT        ConfigParameters_id , Date_Changed, Value_, SUBDIVISION_ID "+
"FROM            PUB.ConfigParameterHST where Date_Changed<? and (ConfigParameters_id=10 or ConfigParameters_id=4 or ConfigParameters_id=5)";*/
            string CommandStr = "SELECT        ConfigParameters_id , Date_Changed, Value_, SUBDIVISION_ID " +
"FROM            staff.staff.PUB.ConfigParameterHST where Date_Changed<p1 and (ConfigParameters_id=10 or ConfigParameters_id=4 or ConfigParameters_id=5)";


            SqlConnection Conn = new SqlConnection(c2Sql);//OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            List<StaffParams> empls = new List<StaffParams>();
            List<StaffParamsTable> Tmp = new List<StaffParamsTable> ();
            SqlCommand Comm = new SqlCommand(CommandStr, Conn);//OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);

            SqlParameter p1 = new SqlParameter("p1", OdbcType.DateTime);//OdbcParameter p1 = new OdbcParameter("p1",OdbcType.DateTime);
            p1.Value = StartDt;
            Comm.Parameters.Add(p1);
            try
            {
                SqlDataReader OdR = Comm.ExecuteReader();//OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {

                        StaffParamsTable st = new StaffParamsTable ()
                        {  
                          CoefType = OdR.GetInt32(0),
                          dtChange = OdR.GetDateTime(1),
                          value = OdR.GetString(2),
                          DepNum = OdR.GetInt32(3)

                        };
                        Tmp.Add(st);
                        

                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.Message);
                    }
                }
                Comm.Dispose();
                Conn.Close();
                foreach (int i in Tmp.Select (a=> a.DepNum ).Distinct())
                {
                    try
                    {
                        //StaffParams Emp = new StaffParams();
                       // Emp.DepNum = i;
                        /*
                        StaffParams t2 = (from o in Tmp
                                          where o.DepNum == i
                                          group o by o.CoefType into vals
                                          let MaxDt = vals.Max(a => a.dtChange)
                                          select new StaffParams()
                                          {
                                              DepNum = i,
                                              NightCoeff = Convert.ToDouble(vals.FirstOrDefault(a => a.CoefType == 10).value),
                                              StartNightStr = vals.FirstOrDefault(a => a.CoefType == 4).value,
                                              StopNightStr = vals.FirstOrDefault(a => a.CoefType == 5).value

                                          }
                                         ).FirstOrDefault();
                        */
                        try
                        {
                            StaffParams t2 = new StaffParams();
                            t2.DepNum = i;
                            DateTime MaxdtNc = Tmp.Where(a => a.CoefType == 10 && a.DepNum == i).Max(a => a.dtChange);
                            string s = Tmp.FirstOrDefault(a => a.DepNum == i && a.CoefType == 10 && a.dtChange == MaxdtNc).value;
                            t2.NightCoeff = Convert.ToDouble(s.Replace(".", ","));


                            MaxdtNc = Tmp.Where(a => a.CoefType == 5 && a.DepNum == i).Max(a => a.dtChange);
                            t2.StartNightStr = Tmp.FirstOrDefault(a => a.DepNum == i && a.CoefType == 5 && a.dtChange == MaxdtNc).value;

                            MaxdtNc = Tmp.Where(a => a.CoefType == 4 && a.DepNum == i).Max(a => a.dtChange);
                            t2.StopNightStr = Tmp.FirstOrDefault(a => a.DepNum == i && a.CoefType == 4 && a.dtChange == MaxdtNc).value;
                        
                        empls.Add(t2);
                        }
                        catch
                        { }
                    }
                    catch(Exception ee)
                    {
                        Console.WriteLine(ee.Message);
                    }

                
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            return empls;

        }
    }

}
