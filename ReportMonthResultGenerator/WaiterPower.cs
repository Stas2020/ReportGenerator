using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
    static class WaiterPower
    {
        static int GetOpenChecksCount(int Dep, DateTime dt)
        {
            Cube2005DataContext Cb = new Cube2005DataContext();
            Cb.CommandTimeout = 0;
            int Res = (from c in Cb.XmlChecks where c.SystemDateOfOpen < dt && c.SystemDate > dt && c.Dep == Dep && c.CompId != 2 && c.CompId != 6 && c.CompId != 54 select c).Count();
            return Res;
        }
        static int GetOpenShortChecksCount(int Dep, DateTime dt, int Period)
        {
            Cube2005DataContext Cb = new Cube2005DataContext();
            Cb.CommandTimeout = 0;
            int Res = (from c in Cb.XmlChecks where ((c.SystemDateOfOpen < dt && c.SystemDate > dt) || (c.SystemDateOfOpen >= dt && c.SystemDate < dt.AddMinutes(Period))) && c.Dep == Dep && c.CompId != 2 && c.CompId != 6 && c.CompId != 54 && c.CheckTimeLong < 240 select c).Count();
            return Res;
        }


        public static void GetPepleCountOfHourToExcel()
        {
            int Dep = 450;
            DateTime StartDate = new DateTime(2018, 10, 01);
            DateTime EndDate = new DateTime(2018, 10, 10);
            List<int> Positions = new List<int>() {5};
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            int dCount = (int)((EndDate - StartDate).TotalDays);

            Ws.Cells[1, 1] = Dep;
            var P = GetPepleCountOfHour(Dep, StartDate, EndDate, Positions);
            for (int h = 0; h < 24; h++)
            {
                Ws.Cells[2, h+1] = h;
                Ws.Cells[3, h + 1] = P[h];
            }

            /*
            int d = 3;
            Dep = 177;
            Ws.Cells[1+d, 1] = Dep;
            P = GetPepleCountOfHour(Dep, StartDate, EndDate, Positions);
            for (int h = 0; h < 24; h++)
            {
                Ws.Cells[2+d, h + 1] = h;
                Ws.Cells[3 + d, h + 1] = P[h];
            }
            
            d += 3;
            Dep = 370;
            Ws.Cells[1 + d, 1] = Dep;
            P = GetPepleCountOfHour(Dep, StartDate, EndDate, Positions);
            for (int h = 0; h < 24; h++)
            {
                Ws.Cells[2 + d, h + 1] = h;
                Ws.Cells[3 + d, h + 1] = P[h];
            };
            */
        }

        public static int[] GetPepleCountOfHour(int Dep, DateTime StartDate, DateTime EndDate, List<int> Positions)
        {
            var Cl1 = new StaffEmpl.StaffEmployeeParametersObjClient();
            var res = new int[24];
            var PositionsId = new List<StaffEmpl.Emploee_working_PositionIDRow>();
            foreach (var Id in Positions)
            {
                PositionsId.Add(new StaffEmpl.Emploee_working_PositionIDRow() { id = Id });
            }
            

            for (var dt = StartDate; dt <= EndDate; dt = dt.AddDays(1))
            {
                for (int h = 0; h < 24; h++)
                {
                    var t = dt.Date.AddHours(h).AddMinutes(30);
                    int? resH = 0;
                    Cl1.Emploee_working(Dep, t, PositionsId.ToArray(),out resH);
                    res[h] += resH.Value;
                }
            }
            return res;
        }


        static public void GenWaiterPower(DateTime dt1, DateTime dt2)
        {
            int Period = 10;

            Utils.ToLog(String.Format("Запуск GenWaiterPower dt1 = {0}, dt2={1} ", dt1, dt2));

            //int Dep=104;

            // List<int> Deps = new List<int>() {230,300,240,212,213};
            //List<int> Deps = new List<int>() { 371,390};
            //List<int> Deps = new List<int>() { 270};

            //    DateTime dt1= new DateTime (2015,7,01); 
            //            DateTime dt2= new DateTime (2015,7,23);
            StaffEmpl.StaffEmployeeParametersObjClient Cl1 = new StaffEmpl.StaffEmployeeParametersObjClient();
            ReportBaseDataContext rb = new ReportBaseDataContext(@"Data Source=s2010;Initial Catalog=Diogen;User ID=v.piskov; Pwd=Eit160t");


            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                try
                {

                    if ((!Dii.Enabled) || (Dii.Place.Replace(" ", "") != "Город")) continue;

                    Utils.ToLog(String.Format("GenWaiterPower Начало расчета подразделение  {0} {1}", Dii.Number, Dii.Name));
                    int Dep = Dii.Number;
                    IEnumerable<ReportWaitersPower> DerRecords = from o in rb.ReportWaitersPowers where o.Dep == Dep && o.dt >= dt1 && o.dt < dt2 select o;

                   
                    rb.ReportWaitersPowers.DeleteAllOnSubmit(DerRecords);
                    rb.SubmitChanges();

                    List<int> Positions = new List<int>() { 27 };
                    var PositionsId = new List<StaffEmpl.Emploee_working_PositionIDRow>();
                    foreach (var Id in Positions)
                    {
                        PositionsId.Add(new StaffEmpl.Emploee_working_PositionIDRow() { id = Id });
                    }


                    for (DateTime dt = dt1; dt < dt2; dt = dt.AddMinutes(Period))
                    {
                        Console.Clear();
                        Console.WriteLine(((dt - dt1).TotalDays * 100 / (dt2 - dt1).TotalDays).ToString("0.00") + "%");

                        int? res = 0;
                        Cl1.Emploee_working(Dep, dt, PositionsId.ToArray(), out res);
                        int ch = GetOpenShortChecksCount(Dep, dt, Period);


                        ReportWaitersPower WP = new ReportWaitersPower
                        {
                            Checks = ch,
                            dt = dt,
                            Waiters = res,
                            Dep = Dep
                        };
                        rb.ReportWaitersPowers.InsertOnSubmit(WP);
                        rb.SubmitChanges();
                    }
                    Utils.ToLog(String.Format("GenWaiterPower Окончание расчета подразделение  {0} {1}", Dii.Number, Dii.Name));
                }

                catch (Exception e)
                {
                    Utils.ToLog("Error GenWaiterPower" + e.Message);
                }
            }
        }
    }
}
