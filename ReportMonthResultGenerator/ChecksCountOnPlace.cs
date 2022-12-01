using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.ServiceModel;
using System.Net;
using System.Data.Odbc;
using System.Data;


namespace ReportMonthResultGenerator
{
    class ChecksCountOnPlace
    {
        public static void GenRep(DateTime dt1, DateTime dt2)
        { 
         Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            
            int col = 3;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 3;
            Ws.Cells[1, 1] = "Количество гостей на одно посадочное место";
            Ws.Cells[2, 1] = "Ресторан";
            Ws.Cells[2, 2] = String.Format("За {0} день", (dt2-dt1).TotalDays);
            Ws.Cells[2, 3] = String.Format("В день" );
            ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                try
                {
                    //int GuestCount = (from o in RepBase.GuestCounts where o.SystemDate >= dt1 && o.SystemDate < dt2 && o.DepNum == Dii.Number  select o.GuestCount1.Value).Sum();
                    int GuestCount = (from o in RepBase.GuestCounts where o.SystemDate >= dt1 && o.SystemDate < dt2 && o.DepNum == Dii.Number select o.Guests.Value).Sum();
                    int PlaceCount = (from o in RepBase.AlohaDepPlaces  where o.DepNum == Dii.Number select o.PosMest.Value).First();
                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = (double)GuestCount / (double)PlaceCount;
                    Ws.Cells[row, 3] = (double)GuestCount / (double)PlaceCount / (double)(dt2 - dt1).TotalDays;
                    row++;
                }
                catch
                { }
            }
            Ws.get_Range("B3:B30").NumberFormat = "0.00";
            Ws.get_Range("C3:C30").NumberFormat = "0.00";

        }



        public static void GenRepMoneyByDay(DateTime dt1, DateTime dt2)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            int col = 3;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 3;
            Ws.Cells[1, 1] = "Количество руб на одно посадочное место";
            Ws.Cells[2, 1] = "Ресторан";
            Ws.Cells[2, 2] = String.Format("За {0} день", (dt2 - dt1).TotalDays);
            Ws.Cells[2, 3] = String.Format("В день");
            BTestDataContext Prod = new BTestDataContext();
            ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                try
                {
                   // int GuestCount = (from o in RepBase.GuestCounts where o.SystemDate >= dt1 && o.SystemDate < dt2 && o.DepNum == Dii.Number select o.GuestCount1.Value).Sum();
                    decimal SumProd = (from o in Prod.Продажиs where o.ДатаВремя >= dt1 && o.ДатаВремя < dt2 && o.КодПодразд == Dii.Number select o.СуммаИтог.Value).Sum();
                    int PlaceCount = (from o in RepBase.AlohaDepPlaces where o.DepNum == Dii.Number select o.PosMest.Value).First();
                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = (double)SumProd / (double)PlaceCount;
                    Ws.Cells[row, 3] = (double)SumProd / (double)PlaceCount / (double)(dt2 - dt1).TotalDays;
                    row++;
                }
                catch
                { }
            }
            Ws.get_Range("B3:B30").NumberFormat = "0.00";
            Ws.get_Range("C3:C30").NumberFormat = "0.00";

        }

        public static List<int> GetPByPos(string Poss)
        { 
            string c2 = "DSN=Staff; Uid=sysprogress;Pwd=progress";
            List<int> Tmp = new List<int>();

            OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();
            string CommandStr = "SELECT PUB.EMPLOYEE.EMPLOYEE_ID " +
                         

"FROM            PUB.EMPLOYEE " +
"WHERE        PUB.EMPLOYEE.POSITION_ID  in (" + Poss + ") ";



            OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);

            OdbcDataReader OdR = Comm.ExecuteReader();
            while (OdR.Read())
            {
                try
                {

                    Tmp.Add( OdR.GetInt32(0));
                   
                    
                }
                catch
                {
                }
            }
            Conn.Close();
            return Tmp;
        }

        public static Dictionary<int,int> FormEmpPoss()
        {
            string c2 = "DSN=Staff; Uid=sysprogress;Pwd=progress";
            Dictionary<int, int> Tmp = new Dictionary<int, int>();

            OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();
            string CommandStr = "SELECT PUB.EMPLOYEE.EMPLOYEE_ID, PUB.EMPLOYEE.POSITION_ID   " +


"FROM            PUB.EMPLOYEE ";




            OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);

            OdbcDataReader OdR = Comm.ExecuteReader();
            while (OdR.Read())
            {
                try
                {

                    Tmp.Add(OdR.GetInt32(0), OdR.GetInt32(1));


                }
                catch
                {
                }
            }
            Conn.Close();
            return Tmp;
        }

        public static void GenRepSalesOnPeople(DateTime dt1, DateTime dt2)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            int col = 3;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 3;
            Ws.Cells[1, 1] = "Выручка на одного сотрудника";
            Ws.Cells[2, 1] = "Ресторан";
            //Ws.Cells[2, 2] = String.Format("За {0} день", (dt2 - dt1).TotalDays);
            //Ws.Cells[2, 3] = String.Format("В день");
            //ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            Cube2005DataContext cb = new Cube2005DataContext();
            BTestDataContext Prod = new BTestDataContext ();

            Prod.CommandTimeout = 0;
            List<int> Tmp = new List<int>();
            vfiliaszar8.Employees emp = new vfiliaszar8.Employees();
            ICredentials credentials = new NetworkCredential("ws", "ws1", "");
            emp.Credentials = credentials;
            string preg =emp.GetPregnantList();
            List<int> Pregs = new List<int>();
            
            foreach (string g in preg.Split(","[0]))
            {
                Pregs.Add(Convert.ToInt32(g));
            }

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                try
                {
                    int Peoples = (from o in cb.StaffEmployees where o.SUBDIVISION_ID==Dii.Number && !Pregs.Contains(o.EMPLOYEE_ID) select o).Count();
                    decimal SumProd = (from o in Prod.Продажиs where o.ДатаВремя >= dt1 && o.ДатаВремя < dt2 && o.КодПодразд == Dii.Number select o.СуммаИтог.Value).Sum();
                    
                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = SumProd ;
                    Ws.Cells[row, 3] =  Peoples;
                    Ws.Cells[row, 4] = SumProd / Peoples;
                  
                    row++;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Ws.get_Range("B3:B30").NumberFormat = "0.00";
            Ws.get_Range("C3:C30").NumberFormat = "0.00";
            Ws.get_Range("D3:D30").NumberFormat = "0.00";

        }

        public static void FotBarista(DateTime dt1, DateTime dt2)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            int col = 3;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 3;
            Cube2005DataContext cb = new Cube2005DataContext();
          
            List<int> Baristas = GetPByPos("5");
            StaffEmpl.Emploee_payRequest req = new StaffEmpl.Emploee_payRequest();
            StaffEmpl.StaffEmployeeParametersObjClient cl = new StaffEmpl.StaffEmployeeParametersObjClient();
            
            

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                try
                {
                    decimal summ=0;
                    for (DateTime dt = dt1; dt < dt2; dt = dt.AddDays(1)) 
                    {
                        StaffEmpl.Emploee_pay_WORKTIME_TEMPRow[] res;
                        try
                        {
                            cl.Emploee_pay(Dii.Number, dt, out res);

                            foreach (StaffEmpl.Emploee_pay_WORKTIME_TEMPRow r in res)
                            {
                                if (Baristas.Contains(r.emploee_id.Value))
                                {

                                    summ += r.PAYROLL.Value;
                                }
                            }
                        }
                        catch(Exception ee)
                        {
                            Console.WriteLine(ee.Message);
                        }
                    }
                   

                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = summ;
                   

                    row++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Ws.get_Range("B3:B30").NumberFormat = "0.00";
            Ws.get_Range("C3:C30").NumberFormat = "0.00";
            Ws.get_Range("D3:D30").NumberFormat = "0.00";

        }

        public static void GetRepFOTPercent(DateTime dt1, DateTime dt2)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            int col = 3;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 3;
            Cube2005DataContext cb = new Cube2005DataContext();
            Dictionary<int, int> EmpPos = FormEmpPoss();
            
            StaffEmpl.Emploee_payRequest req = new StaffEmpl.Emploee_payRequest();
            StaffEmpl.StaffEmployeeParametersObjClient cl = new StaffEmpl.StaffEmployeeParametersObjClient();
            //Ws.Cells[1, 1] = "Выручка на одного сотрудника";
            //Ws.Cells[2, 1] = "Ресторан";
            //Ws.Cells[2, 2] = String.Format("За {0} день", (dt2 - dt1).TotalDays);
            //Ws.Cells[2, 3] = String.Format("В день");
            //ReportBaseDataContext RepBase = new ReportBaseDataContext("Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov;Password=Eit160t");
            List<int> Pos1 = new List<int>() {2,4,8,20 };
            List<int> Pos2 = new List<int>() { 5,6,12};
            List<int> Pos3 = new List<int>() { 18};
            Ws.Cells[1, 1] = "Подразделение";
            Ws.Cells[1, 2] = "Кухня";
            Ws.Cells[1, 3] = "Стойка";
            Ws.Cells[1, 4] = "Остальные";
                    
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                //if (Dii.Number!=104) continue;
                try
                {
                    decimal summ = 0;
                    decimal summ1 = 0;
                    decimal summ2 = 0;
                    decimal summ3 = 0;
                    
                    
                    for (DateTime dt = dt1; dt < dt2; dt = dt.AddDays(1))
                    {
                        StaffEmpl.Emploee_pay_WORKTIME_TEMPRow[] res;
                        try
                        {
                            cl.Emploee_pay(Dii.Number, dt, out res);
                            foreach (StaffEmpl.Emploee_pay_WORKTIME_TEMPRow r in res)
                            {
                    
                                    summ += r.PAYROLL.Value;
                                int pos= EmpPos[r.emploee_id.Value];
                                if (Pos1.Contains(pos))
                                {
                                    summ1 += r.PAYROLL.Value;
                                }
                                if (Pos2.Contains(pos))
                                {
                                    summ2 += r.PAYROLL.Value;
                                }
                                if (Pos3.Contains(pos))
                                {
                                    summ3 += r.PAYROLL.Value;
                                }
                                
                            }
                        }
                        catch (Exception ee)
                        {
                            Console.WriteLine(ee.Message);
                        }
                    }
                    //int Peoples = (from o in cb.StaffEmployees where o.SUBDIVISION_ID == Dii.Number && !Pregs.Contains(o.EMPLOYEE_ID) select o).Count();
                    //decimal SumProd = (from o in Prod.Продажиs where o.ДатаВремя >= dt1 && o.ДатаВремя < dt2 && o.КодПодразд == Dii.Number select o.СуммаИтог.Value).Sum();

                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = summ1/summ;
                    Ws.Cells[row, 3] = summ2/summ;
                    Ws.Cells[row, 4] = 1 - summ1 / summ - summ2 / summ;
                    //Ws.Cells[row, 5] = summ;
                    row++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Ws.get_Range("B3:B30").NumberFormat = "0.00 %";
            Ws.get_Range("C3:C30").NumberFormat = "0.00 %";
            Ws.get_Range("D3:D30").NumberFormat = "0.00 %";

        }
    }
}
