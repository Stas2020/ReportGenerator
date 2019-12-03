using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;



namespace ReportMonthResultGenerator
{
    static class DismissalPercent
    {
        static string c2 = "DSN=Staff; Uid=sysprogress;Pwd=progress";
         static OdbcConnection Conn;


        

        internal static List<StaffDismissal> GetStaffDismissal(DateTime Fdt, DateTime Edt)
        {
            List<StaffDismissal> Tmp = new List<StaffDismissal>();

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Conn = new OdbcConnection(c2);
            Conn.Open();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                StaffDismissal Sd = new StaffDismissal
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name,
                    DismissCount = GetDismissCount(Dii.Number, Fdt, Edt),
                    StaffCount = GetStaffCount(Dii.Number)
                };
                Tmp.Add(Sd);
            }
            Conn.Close();
            return Tmp;

        }


        internal static int GetDismissCount(int Dn, DateTime FDt, DateTime EDt)
        {
            string CommandStr = "SELECT  Count(*)  FROM PUB.Employee where PUB.Employee.SUBDIVISION_ID=? and dismissal_date>=? and dismissal_date<?";


            
            

            OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            OdbcParameter P0 = new OdbcParameter("P1", OdbcType.Int);
            P0.Value = Dn;
            OdbcParameter P1 = new OdbcParameter("P1", OdbcType.Date);
            P1.Value = FDt.Date;
            OdbcParameter P2 = new OdbcParameter("P2", OdbcType.Date);
            P2.Value = EDt.Date;
            Comm.Parameters.Add(P0);
            Comm.Parameters.Add(P1);
            Comm.Parameters.Add(P2);


            int res = -1;
            try
            {
                OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        res = OdR.GetInt32(0);

                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {


            }

            
            return res;
        }
        static internal int GetStaffCount(int Dn)
        {
            string CommandStr = "SELECT  Count(*)  FROM PUB.Employee where PUB.Employee.SUBDIVISION_ID=? and dismissal_date is null ";


            
            

            OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);
            OdbcParameter P0 = new OdbcParameter("P1", OdbcType.Int);
            P0.Value = Dn;

            Comm.Parameters.Add(P0);



            int res = -1;
            try
            {
                OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        res = OdR.GetInt32(0);

                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {


            }

            
            return res;
        }

    }
}
