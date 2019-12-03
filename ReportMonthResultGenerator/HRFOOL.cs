using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Data.Odbc;
using System.Data;
using System.Globalization;
using System.IO;

namespace ReportMonthResultGenerator
{
    public class EmplSal
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Smen { set; get; }
        public decimal Hours { set; get; }
        public string Stavka { set; get; }
        public decimal Zta { set; get; }
        public decimal Prem { set; get; }
        public decimal Visl { set; get; }
        public decimal Vir { set; get; }
        public decimal AllSumm { set; get; }

    }

    public static class HRFOOL
    {

        public static void GetLastPosRep()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            Cube2005DataContext data = new Cube2005DataContext();
            int row = 2;

            IQueryable<StaffEmployeeEx> Empls = data.StaffEmployeeEx.Where(a => a.DISMISSAL_DATE==null);
            foreach (StaffEmployeeEx SE in Empls)
            {
                Ws.Cells[row, 1] = SE.LAST_NAME+" "+SE.FIRST_NAME+" "+SE.MIDDLE_NAME;
                Ws.Cells[row, 2] = SE.EMPLOYEE_ID;
                Ws.Cells[row, 3] = GetLastPos(SE.EMPLOYEE_ID, SE.ENTRY_DATE.Value, SE.POSITION_ID.Value).ToString("dd/MM/yyyy");
                row++;
            }

        }


        public static void GetLastPosRepByPos(int PosId)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            Cube2005DataContext data = new Cube2005DataContext();
            int row = 2;

            IQueryable<StaffEmployeeEx> Empls = data.StaffEmployeeEx.Where(a => a.DISMISSAL_DATE == null && a.POSITION_ID== PosId);
            foreach (StaffEmployeeEx SE in Empls)
            {
                Ws.Cells[row, 1] = SE.LAST_NAME + " " + SE.FIRST_NAME + " " + SE.MIDDLE_NAME;
                Ws.Cells[row, 2] = SE.EMPLOYEE_ID;
                DateTime LPD = GetLastPos(SE.EMPLOYEE_ID, SE.ENTRY_DATE.Value, SE.POSITION_ID.Value);
                Ws.Cells[row, 3] = LPD.ToString("dd/MM/yyyy");
                Ws.Cells[row, 4] = (int)(DateTime.Now- LPD).TotalDays;
                row++;
            }

        }

        private static DateTime GetLastPos(int Id, DateTime def,int CurentPosId)
        {
            Staff.procedure_for_history_t_emphistoryRow[] Tmp;
            Staff.StaffEmployeeParametersObjClient s1 = new Staff.StaffEmployeeParametersObjClient();
            string res = s1.procedure_for_history(Id, out Tmp);
            if ((Tmp == null) || (Tmp.Count() == 0)) return def;
            DateTime LastVal = Tmp[0].work_from_date.Value;
            foreach (Staff.procedure_for_history_t_emphistoryRow h in Tmp.OrderByDescending(a=>a.work_from_date))
            {
                if (h.position_id != CurentPosId)
                {
                    return LastVal;
                }
                LastVal = h.work_from_date.Value;
            }
            return LastVal;
            //DateTime dtOut = Tmp.ToList().Select(a => a.work_from_date).Max().Value;

            //return dtOut;
            
        }

        public static decimal GetStavka(string In)
        {
            string In2 = In.Trim();
            decimal lastval = 0;

            for (int i = 1; i < In2.Length; i++)
            {
                decimal l;
                if (decimal.TryParse(In2.Substring(0, i).Replace(".", ","), out l))
                {
                    lastval = l;
                }
                else
                {
                    break;
                }
            }
            return lastval;
        }

        public static List<EmplSal> GetEmpSals(List<int> data)
        {
            List<EmplSal> res = new List<EmplSal>();
            DirectoryInfo DI = new DirectoryInfo(@"d:\VED");
            foreach (FileInfo fi in DI.GetFiles())
            {
                Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook Wb = app.Workbooks.Open(fi.FullName);
                Microsoft.Office.Interop.Excel.Worksheet Ws = Wb.Worksheets[1];
                for (int row = 11; row < 300; row++)
                {
                    try
                    {

                        if (Ws.Cells[row, 1].value==null || Ws.Cells[row, 1].value.ToString() == "") continue;
                        EmplSal S = new EmplSal();
                        if (Ws.Cells[row, 1].value != null)
                        {
                            S.Id = Convert.ToInt32(Ws.Cells[row, 1].value);
                        }
                        if (Ws.Cells[row, 2].value != null)
                        {
                            S.Name = Ws.Cells[row, 2].value;
                        }
                        if (Ws.Cells[row, 3].value != null)
                        {
                            //S.Stavka = GetStavka(Ws.Cells[row, 3].value);
                            S.Stavka = Ws.Cells[row, 3].value.ToString();
                        }
                        if (Ws.Cells[row, 4].value != null)
                        {
                            S.Smen = Convert.ToDecimal(Ws.Cells[row, 4].value);
                        }
                        
                        if (Ws.Cells[row, 5].value !=null)
                        {
                            S.Hours = Convert.ToDecimal(Ws.Cells[row, 5].value);
                        }
                        if (Ws.Cells[row, 7].value != null)
                        {
                            S.Zta = Convert.ToDecimal(Ws.Cells[row, 7].value);
                        }
                        if (Ws.Cells[row, 8].value != null)
                        {
                            S.Prem = Convert.ToDecimal(Ws.Cells[row, 8].value);
                        }
                        if (Ws.Cells[row, 9].value != null)
                        {
                            S.Visl = Convert.ToDecimal(Ws.Cells[row, 9].value);
                        }
                        /*
                        if ((Ws.Cells[row, 18].value != null)&& (Ws.Cells[row, 20].value != null))
                        {
                            S.AllSumm = Convert.ToDecimal(Ws.Cells[row, 18].value) + Convert.ToDecimal(Ws.Cells[row, 20].value);
                        }
                        */
                        decimal Av = 0;
                        decimal Z = 0;

                        if ((Ws.Cells[row, 18].value != null) )
                        {
                            Av = Convert.ToDecimal(Ws.Cells[row, 18].value);
                        }
                        if ((Ws.Cells[row, 20].value != null))
                        {
                            Z = Convert.ToDecimal(Ws.Cells[row, 20].value);
                        }
                        S.AllSumm = Av + Z;
                        res.Add(S);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error " + e.Message);
                    }



                }
            }
            List<EmplSal> Outres = new List<EmplSal>();
            foreach (int EmplId in data)
            {
                if (res.Where(a => a.Id == EmplId).Count() > 0)
                {
                    EmplSal S = new EmplSal();
                    Outres.Add(S);
                    List<EmplSal> AllEmplWt = res.Where(a => a.Id == EmplId).ToList();
                    S.Hours = AllEmplWt.Sum(a => a.Hours);
                    S.Id = EmplId;
                    S.Name = AllEmplWt[0].Name;
                    S.Prem = AllEmplWt.Sum(a => a.Prem);
                    S.Smen = AllEmplWt.Sum(a => a.Smen);
                    S.Stavka = AllEmplWt[0].Stavka;
                    
                    S.Vir = AllEmplWt.Sum(a => a.Vir);
                    S.Visl = AllEmplWt.Sum(a => a.Visl);
                    S.Zta = AllEmplWt.Sum(a => a.Zta);
                    S.AllSumm = AllEmplWt.Sum(a => a.AllSumm);
                }


            }

            return Outres;

        }

        public static void GetRepChik()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            // List<int> DepsAir = new List<int>() {212, 213, 214, 216, 217, 222,230,231,300 };
            List<int> DepsCityCoff = new List<int>() { 101, 104, 130, 177, 205, 260, 270, 295, 310, 370, 371, 375, 380, 390, 395, 290, 285 };
            Cube2005DataContext data = new Cube2005DataContext();
            int row = 2;

            List<int> Empls = data.StaffEmployeeEx.Where(a => DepsCityCoff.Contains(a.SUBDIVISION_ID.Value) && ((a.POSITION_ID == 5) ||(a.POSITION_ID == 6) || (a.POSITION_ID == 12))).Select(a => a.EMPLOYEE_ID).ToList();
            List<EmplSal> EmplSalData = GetEmpSals(Empls);
            decimal summ = EmplSalData.Sum(a => a.AllSumm);

        }

        public static void GetRepLitv()
        {

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
           // List<int> DepsAir = new List<int>() {212, 213, 214, 216, 217, 222,230,231,300 };
            List<int> DepsCity = new List<int>() { 101,104,130,177,180,205,255,260,264,270,295,310,370,371,375,380,390,395,280,290,285,320};
            Cube2005DataContext data = new Cube2005DataContext();
            int row = 2;

            List<int> Empls = data.StaffEmployeeEx.Where(a => DepsCity.Contains(a.SUBDIVISION_ID.Value) && a.DISMISSAL_DATE == null).Select(a => a.EMPLOYEE_ID).ToList();
            List<EmplSal> EmplSalData = GetEmpSals(Empls);
            foreach (StaffDepartments Dep in data.StaffDepartments)
            {
                if (!DepsCity.Contains(Dep.DepID)) continue;
                //Ws.Cells[row, 2] = Dep.DepName;
                //row++;

                foreach (StaffEmployeeEx Empl in data.StaffEmployeeEx.Where(a => a.SUBDIVISION_ID == Dep.DepID && a.DISMISSAL_DATE == null))
                {

                    if (EmplSalData.Where(a => a.Id == Empl.EMPLOYEE_ID).Count() == 0) continue;

                    EmplSal S = EmplSalData.Where(a => a.Id == Empl.EMPLOYEE_ID).First();
                    Ws.Cells[row, 1] = Dep.DepName; 
                    Ws.Cells[row, 2] = S.Id;
                    Ws.Cells[row, 3] = S.Name.Replace("*","");
                    Ws.Cells[row, 4] = data.Staffposition.Where(a=>a.POSITION_ID==Empl.POSITION_ID).First().NAME;
                    Ws.Cells[row, 5] = S.Stavka;
                    Ws.Cells[row, 6] = S.Smen;
                    Ws.Cells[row, 7] = S.Hours;
                    Ws.Cells[row, 8] = S.Zta;
                    Ws.Cells[row, 9] = S.Prem;
                    Ws.Cells[row, 10] = S.AllSumm;
                    

                    row++;
                }
                row++;
            }
            Ws.get_Range("A1:B1").EntireColumn.AutoFit();
        }

        

        

        public static void GetRepDissmiss()
        {
            DateTime StartDt = new DateTime(2017, 1, 1);

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            Staff.StaffEmployeeParametersObjClient Cl = new Staff.StaffEmployeeParametersObjClient();

            Cube2005DataContext data = new Cube2005DataContext();
            int row = 2;
            foreach (StaffDepartments Dep in data.StaffDepartments)
            {
                bool x = false;
                Ws.Cells[row, 1] = Dep.DepName;
                
                foreach (Staffposition Sp in data.Staffposition)
                {
                    
                    int In = data.StaffEmployeeEx.Where(a => a.ENTRY_DATE >= StartDt && a.POSITION_ID == Sp.POSITION_ID && a.SUBDIVISION_ID == Dep.DepID).Count();
                    int Out = data.StaffEmployeeEx.Where(a => a.DISMISSAL_DATE != null && a.DISMISSAL_DATE >= StartDt && a.POSITION_ID == Sp.POSITION_ID && a.SUBDIVISION_ID == Dep.DepID).Count();
                    int OutOfIn = data.StaffEmployeeEx.Where(a => a.DISMISSAL_DATE != null && a.ENTRY_DATE >= StartDt && a.DISMISSAL_DATE >= StartDt && a.POSITION_ID == Sp.POSITION_ID && a.SUBDIVISION_ID == Dep.DepID).Count();

                    if (In + Out + OutOfIn > 0)
                    {
                        row++;
                        Ws.Cells[row, 2] = Sp.NAME;
                        Ws.Cells[row, 3] = In;
                        Ws.Cells[row, 4] = Out;
                        Ws.Cells[row, 5] = OutOfIn;
                        
                        x = true;
                    }
                }
                if (x)
                {
                    row++;
                    row++;
                }

            }
            Ws.get_Range("A1:B1").EntireColumn.AutoFit();

        }


        public static void GetRep()
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            Staff.StaffEmployeeParametersObjClient Cl = new Staff.StaffEmployeeParametersObjClient();

            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            List<EmplDissm> El = GetEmpList(new DateTime(2017, 01, 01));
            List<int> pp = new List<int>();
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name).Where(a => a.Enabled = true && a.Kassez.Count > 0))
            {
                if (pp.Contains(Dii.Number) || (Dii.Number == 175))
                {
                    continue;
                }
                pp.Add(Dii.Number);
                Ws.Name = Dii.Name;
                var Data = El.Where(a => a.Dep == Dii.Number);
                int row = 2;

                CultureInfo ci = new CultureInfo("en-US");
                DateTimeFormatInfo dtfi = ci.DateTimeFormat;

                for (int i = 1; i <= 12; i++)
                {
                    Ws.Cells[1, i + 1] = dtfi.MonthGenitiveNames[i - 1];
                }

                foreach (string Pos in Data.Select(a => a.Pos).Distinct())
                {
                    Ws.Cells[row, 1] = Pos;
                    for (int i = 1; i <= 12; i++)
                    {
                        Ws.Cells[row, i + 1] = Data.Where(a => a.Pos == Pos && a.Month == i).Count();
                    }
                    row++;
                }
                Ws.Cells[row + 1, 1] = Dii.Name;
                Ws.get_Range("A1:A1").EntireColumn.AutoFit();
                Ws = Wb.Worksheets.Add();
            }
        }
        private static List<EmplDissm> GetEmpList(DateTime EDt)
        {
            List<EmplDissm> Tmp = new List<EmplDissm>();

            string c2 = "DSN=Staff; Uid=sysprogress;Pwd=progress";
            string CommandStr = "SELECT  PUB.EMPLOYEE.EMPLOYEE_ID, PUB.EMPLOYEE.DISMISSAL_DATE,  PUB.POSITION.NAME  AS Expr2, PUB.EMPLOYEE.SUBDIVISION_ID  " +
                " FROM PUB.EMPLOYEE, PUB.POSITION, PUB.SUBDIVISION  " +
                "WHERE        PUB.EMPLOYEE.POSITION_ID = PUB.POSITION.POSITION_ID AND PUB.EMPLOYEE.SUBDIVISION_ID = PUB.SUBDIVISION.SUBDIVISION_ID " +
                " and PUB.EMPLOYEE.dismissal_date>=? ";


            OdbcConnection Conn = new OdbcConnection(c2);
            Conn.Open();

            OdbcCommand Comm = new OdbcCommand(CommandStr, Conn);


            OdbcParameter P2 = new OdbcParameter("P2", OdbcType.Date);
            P2.Value = EDt.Date;
            Comm.Parameters.Add(P2);

            try
            {
                OdbcDataReader OdR = Comm.ExecuteReader();

                while (OdR.Read())
                {
                    try
                    {
                        EmplDissm Ed = new EmplDissm()
                        {
                            Id = OdR.GetInt32(0),
                            Dep = OdR.GetInt32(3),
                            Month = OdR.GetDate(1).Month,
                            Pos = OdR.GetString(2),
                        };
                        Tmp.Add(Ed);
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {


            }

            Conn.Close();
            return Tmp;
        }
    }
    public class EmplDissm
    {
        public EmplDissm()
        { }
        public int Id { set; get; }
        public string Pos { set; get; }
        public int Dep { set; get; }
        public int Month { set; get; }

    }
}


