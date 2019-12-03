using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace ReportMonthResultGenerator
{
    class StaffWtToExcel
    {

        public static void AvgStavka(DateTime Month)
        {

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;

            Staff.StaffEmployeeParametersObjClient Cl = new Staff.StaffEmployeeParametersObjClient();

            int Pos = 8;
            Cube2005DataContext cb = new Cube2005DataContext();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            List<int> StPos = new List<int>() { Pos };
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, StPos), Month, Month.AddMonths(1));
            int row = 1;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                decimal SalSum = 0;
                if (!Dii.Enabled) continue;
                List<int> pp = cb.StaffEmployees.Where(a => a.Position_ID == Pos && a.SUBDIVISION_ID == Dii.Number).Select(a => a.EMPLOYEE_ID).ToList();
                foreach (int p in pp)
                {
                    decimal? sal = 0;
                    Cl.person_salary(p, Dii.Number, Pos, 3, 2017, out sal);
                    SalSum += sal.Value;
                }
                Double Wh = Wts.Where(a => pp.Contains(a.Emp.Id) && a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                double res = (double)SalSum / Wh;

                Ws.Cells[row, 1] = Dii.Name; //Ресторан
                try
                {
                    Ws.Cells[row, 2] = SalSum; //Кол-во блюд
                    Ws.Cells[row, 3] = Wh; //Выручка от блюд
                    Ws.Cells[row, 4] = res; //Выручка от блюд
                }
                catch
                { }
                row++;
            }




        }


        public static void GenrepForStoika(DateTime Month)
        {
            List<int> StPos = new List<int>() { 12, 6 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            //Ws.Cells[1, 2] = "Критерии";

            int col = 3;

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, StPos), Month, Month.AddMonths(1)); //Рабочие смены по стойке
            Wts.AddRange(StaffBase.GetWts(StaffBase.getPeopleOfPosOld(" 12 ", Month), Month, Month.AddMonths(1))); //Рабочие смены по стойке)
            Wts.AddRange(StaffBase.GetWts(StaffBase.getPeopleOfPosOld(" 6 ", Month), Month, Month.AddMonths(1))); //Рабочие смены по стойке)
            int row = 2;

            List<DishCount> Dk = CubeData.GetStoikaDishesCount(Month);


            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;


                double HoursCount = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 4).Count(); // Всего смен в день более 4 часов

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList(); //Сотрудники

                // List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                //     StaffEmpl.division_pay_EMPLOEE_PARAMRow[] emppar = new StaffEmpl.division_pay_EMPLOEE_PARAMRow[0];
                //     decimal? res2 = 0;
                //     Cl.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                //     string s = Cl.division_pay(Dii.Number, Month, Pos.ToArray(), out res2, out emppar);

                StaffEmpl.StaffEmployeeParametersObjClient Cl = new StaffEmpl.StaffEmployeeParametersObjClient();
                Cl.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                decimal summFot = 0;
                foreach (int emp2 in Emps)
                {

                    //foreach (int P in StPos)
                    {
                        int P = Wts.FirstOrDefault(a => a.Dep == Dii.Number && (a.Emp.Id == emp2)).Emp.Pos;
                        decimal? ress = 0;
                        string ss = Cl.person_salary(emp2, Dii.Number, P, Month.Month, Month.Year, out ress);
                        if (ress != null)
                        {
                            summFot += ress.Value;
                        }
                    }




                }



                Ws.Cells[row, 1] = Dii.Name; //Ресторан
                try
                {
                    Ws.Cells[row, 2] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).Count; //Кол-во блюд
                    Ws.Cells[row, 3] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount; //Выручка от блюд
                }
                catch
                { }
                Ws.Cells[row, 4] = summFot; //ФОТ
                Ws.Cells[row, 5] = CountInDay; //Смен всего
                Ws.Cells[row, 5] = HoursCount; //Часов всего






                row++;

            }






            //   app.Save(System.Reflection.Missing.Value);

            //  app = null;
        }

        public static void GenrepDeserts(DateTime Month)
        {

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            //Ws.Cells[1, 2] = "Критерии";
            List<DishCount> Dk = CubeData.GetStoikaDesertCount(Month);
            List<DishCount> AllDk = CubeData.GetAllDishesCount(Month);

            int col = 3;


            try
            {

                Ws.Cells[1, 3] = Dk.Sum(a => a.MoneyCount);
                Ws.Cells[2, 3] = AllDk.Sum(a => a.MoneyCount);
            }
            catch
            { }







        }


        public static void KithenDishCount(DateTime Month)
        {
            List<int> KPos = new List<int>() { 2, 8 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;

            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);

            int row = 2;
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).Count;
                //Ws.Cells[row, 3] = CountInDay / 31;
                //Ws.Cells[row, 4] = AllEmpl.Sum(a => a.sal)*30;
                //Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                row++;

            }


            app.Save(System.Reflection.Missing.Value);

            app = null;
        }


        public static void GenDecoratorsRep2(DateTime Month)
        {

            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();


            List<int> Poss = new List<int>() { 7, 11, 23, 33, 42, 44, 48 };
            List<int> Decorators = new List<int>() { 8622, 1099, 11423, 2718, 2536, 10132, 4542, 1524, 1109, 4729 };

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;

            Month = new DateTime(2015, 3, 1);
            int row = 1;
            StaffEmpl.StaffEmployeeParametersObjClient Cl = new StaffEmpl.StaffEmployeeParametersObjClient();

            //  List<CEmplWt> Wts = StaffBase.GetWts(new List<CEmpl> { new CEmpl{Id=1099}}, Month, Month.AddMonths(1));

            decimal? res = 0;
            // string ss = Cl.person_salary(517, 104, 19, 3, 2015, out res);

            foreach (int e in Decorators)
            {
                decimal summ = 0;
                foreach (int Pos in Poss)
                {
                    res = 0;
                    string ss = Cl.person_salary(e, 103, Pos, 3, 2015, out res);
                    summ += res.Value;
                }
                Ws.Cells[row, 1] = e;
                Ws.Cells[row, 2] = summ;

                //Ws.Cells[row, 3] = CountInDay / 31;
                //Ws.Cells[row, 4] = AllEmpl.Sum(a => a.sal)*30;
                //Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                row++;
            }



            app.Save(System.Reflection.Missing.Value);

            app = null;
        }

        public static void GenRepkassirwt(DateTime Month)
        {
            List<int> KPos = new List<int>() { 19 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;


            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 2;



            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                //if (Dii.Number != 205) continue;
                //StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);      
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                Ws.Cells[row, 1] = Dii.Name;

                Ws.Cells[row, 3] = res;


                row++;

            }


        }

        public static Dictionary<int, double> GetStoykaPercent(DateTime Month)
        {
            Dictionary<int, double> Tmp = new Dictionary<int, double>();
            List<int> KPos = new List<int>() { 12, 5, 6 };
            //List<int> KPos = new List<int>() {   6 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();


            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));

            List<DishCount> Dk = CubeData.GetStoikaDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                //if (Dii.Number == 260) res -= 264.75;

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();
                Utils.ToLog(String.Format("StPer Dep: {0}, D: {1}, H: {2}", Dii.Number, Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count).ToString(), res.ToString()));
                Tmp.Add(Dii.Number, (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count) / res);

            }

            return Tmp;
        }


        public static Dictionary<int, double> GetKitchenPercent(DateTime Month)
        {
            Dictionary<int, double> Tmp = new Dictionary<int, double>();
            List<int> KPos = new List<int>() { 2, 8 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();


            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));

            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                if (Dii.Number == 260) res -= 387.2;

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Tmp.Add(Dii.Number, (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count) / res);

            }

            return Tmp;
        }

        public static void GenRepAll(DateTime Month)
        {
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            List<CEmpl> AllEmpl = StaffBase.GetAllEmpl(Month, Month.AddMonths(1));
            List<CEmplWt> Wts = StaffBase.GetWts(AllEmpl.Where(a => a.Pos != 12 && a.Pos != 18 && a.Pos != 27).ToList(), Month, Month.AddDays(27));
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            int row = 1;
            Dictionary<int, int> ChCount = сRashMat.GetChkCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddDays(27))).TotalHours);
                //if (Dii.Number == 260) res -= 264.75;

                //int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                //int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                //List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                //List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                //Tmp.Add(Dii.Number, (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count) / res);




                Ws.Cells[row, 1] = Dii.Name;
                int ChCounts = 0;
                ChCount.TryGetValue(Dii.Number, out ChCounts);
                Ws.Cells[row, 3] = res;
                Ws.Cells[row, 4] = ChCounts;
                row++;
            }
        }
        
        public static Dictionary<int, decimal> GetWtsNoWaiter(DateTime Month)
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            //var chkCount = CubeData.GetChecksCount(Month);
            Dictionary<int, decimal> resOut = new Dictionary<int, decimal>();
            var staffSrv = new StaffEmpl.StaffEmployeeParametersObjClient();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                StaffEmpl.working_time_T_wrkRow[] emplres = new StaffEmpl.working_time_T_wrkRow[200];
                var res = staffSrv.working_time(Month, Month.AddMonths(1), Dii.Number, 0, out emplres); //StaffBase.GetWtsByDep(Dii.Number, Month, Month.AddMonths(1)).Where(a=>a.Emp.Pos!=27);
                var Wts = emplres.Where(a => a.p_id != 27);
                decimal wtsCount = (decimal)Wts.Sum(a => (StaffWtToExcel.GetMinDate(a.dtm_fn.GetValueOrDefault(), Month.AddMonths(1))- StaffWtToExcel.GetMaxDate(a.dtm_st.GetValueOrDefault(), Month)).TotalHours);
                //int mchkCount = 0;
                resOut.Add(Dii.Number, wtsCount);
                //chkCount.TryGetValue(Dii.Number, out mchkCount);
            }
            return resOut;
        }

        public static Dictionary<int, decimal> GetWtsNoWaiterByDay(DateTime day)
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            //var chkCount = CubeData.GetChecksCount(Month);
            Dictionary<int, decimal> resOut = new Dictionary<int, decimal>();
            var staffSrv = new StaffEmpl.StaffEmployeeParametersObjClient();
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                StaffEmpl.working_time_T_wrkRow[] emplres = new StaffEmpl.working_time_T_wrkRow[200];
                var res = staffSrv.working_time(day, day.AddDays(1), Dii.Number, 0, out emplres); //StaffBase.GetWtsByDep(Dii.Number, Month, Month.AddMonths(1)).Where(a=>a.Emp.Pos!=27);
                var Wts = emplres.Where(a => a.p_id != 27);
                decimal wtsCount = (decimal)Wts.Sum(a => (StaffWtToExcel.GetMinDate(a.dtm_fn.GetValueOrDefault(), day.AddDays(1)) - StaffWtToExcel.GetMaxDate(a.dtm_st.GetValueOrDefault(), day)).TotalHours);
                //int mchkCount = 0;
                resOut.Add(Dii.Number, wtsCount);
                //chkCount.TryGetValue(Dii.Number, out mchkCount);
            }
            return resOut;
        }



        public static Dictionary<int, decimal> GetWtsNoWaiter(DateTime d1, DateTime d2)
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            //var chkCount = CubeData.GetChecksCount(Month);
            Dictionary<int, decimal> resOut = new Dictionary<int, decimal>();
            var staffSrv = new StaffEmpl.StaffEmployeeParametersObjClient();
            staffSrv.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                StaffEmpl.working_time_T_wrkRow[] emplres = new StaffEmpl.working_time_T_wrkRow[200];
                var res = staffSrv.working_time(d1, d2, Dii.Number, 0, out emplres); //StaffBase.GetWtsByDep(Dii.Number, Month, Month.AddMonths(1)).Where(a=>a.Emp.Pos!=27);
                var Wts = emplres.Where(a => a.p_id != 27);
                decimal wtsCount = (decimal)Wts.Sum(a => (StaffWtToExcel.GetMinDate(a.dtm_fn.GetValueOrDefault(), d2) - StaffWtToExcel.GetMaxDate(a.dtm_st.GetValueOrDefault(), d1)).TotalHours);
                //int mchkCount = 0;
                resOut.Add(Dii.Number, wtsCount);
                //chkCount.TryGetValue(Dii.Number, out mchkCount);
            }
            return resOut;
        }

        public static void GenRepKitchen(DateTime Month)
        {
            List<int> KPos = new List<int>() { 2, 8 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            Worksheet Ws2 = Wb.Worksheets.Add();
            Ws2.Name = "Монеты";

            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;

            Ws.Cells[1, 1] = "Подразделение";
            Ws.Cells[1, 2] = "Кол-во блюд";
            Ws.Cells[1, 3] = "Человеко-часы";
            Ws.Cells[1, 4] = "Блюд в человеко-час";

            Ws2.Cells[1, 1] = "Подразделение";
            Ws2.Cells[1, 2] = "Кол-во монет";
            Ws2.Cells[1, 3] = "Человеко-часы";
            Ws2.Cells[1, 4] = "Монет в человеко-час";
            
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 2;

            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
//if (Dii.Number != 205) continue;
//StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);      
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
            //    if (Dii.Number == 260) res -= 264.75;

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();
                //List<CEmpl> AllEmpl = StaffBase.GetAllSal(Month, Emps);

              /*
                    foreach (CEmpl emp2 in AllEmpl.OrderBy(a => a.Id))
                    {
                        List<CEmplWt> WtsOfEmpl = Wts.Where(a => a.Dep == Dii.Number && a.Emp.Id == emp2.Id).ToList();
                                                Ws.Cells[row, 1] = emp2.Id;
                        //Ws.Cells[row, 2] = emp2.Money;
                        Ws.Cells[row, 4] = WtsOfEmpl.Sum(a => GetNightHours(SP, a));
                        Ws.Cells[row, 5] = WtsOfEmpl.Sum(a => (a.StopDt - a.StartDt).TotalHours);
                       // Ws.Cells[row, 4] = emp2.DayHourCount;
                       // Ws.Cells[row, 5] = emp2.NighHourCount;
                        row++;
                    }
                */


                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a=>a.Dep==Dii.Number).Sum(a=>a.Count);
                Ws.Cells[row, 3] = res;
                Ws.Cells[row, 4] = (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count)/res;

                Ws2.Cells[row, 1] = Dii.Name;
                Ws2.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.MoneyCount);
                Ws2.Cells[row, 3] = res;
                Ws2.Cells[row, 4] = (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.MoneyCount)/res;


                //Ws.Cells[row, 3] = CountInDay / 31;
                //Ws.Cells[row, 4] = AllEmpl.Sum(a => a.sal)*30;
                //Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                row++;

            }


       //     app.Save(System.Reflection.Missing.Value);

      //      app = null;
        }

        /*
        public List<CEmplWt> RemoveNightHours(List<CEmplWt>  OriginalTimes,int Dep)
        {
            int StartNight = 23;
            int StopNight = 7;
            List<CEmplWt> Tmp = new List<CEmplWt>();
            foreach (CEmplWt Wt in OriginalTimes)
            {
                if (Wt.Dep == Dep)
                {
                    if ((Wt.StartDt.Hour < StopNight || Wt.StartDt.Hour >= StartNight) && (Wt.StopDt.Hour < StopNight) || (Wt.StopDt.Hour >= StartNight))
                    {
                        continue;
                    }
                    if (Wt.StartDt.Hour < StopNight || Wt.StartDt.Hour >= StartNight)
                    {
                        Wt.StartDt = (Wt.StartDt.Hour >= StartNight) ? Wt.StartDt.Date.AddDays(1).AddHours(StopNight) : Wt.StartDt.Date.AddHours(StopNight);
                    }


                    }
                }
            }
        }
        */
        public static void GenRepKitchenTmp(DateTime Month)
        {
            //Повар, су-шеф, старший повар
            List<int> KPos = new List<int>() {2,8,4 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            int col = 3;

            Ws.Cells[1, 1] = "Подразделение";
            Ws.Cells[1, 2] = "Кол-во блюд";
            Ws.Cells[1, 3] = "Процент просрочки";
            Ws.Cells[1, 4] = "Кол-во хороших блюд";
            Ws.Cells[1, 5] = "Человеко-часы";
            Ws.Cells[1, 6] = "Блюд в человеко-час";
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 2;

            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);

            //   foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();
            bool c = true;
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim().ToLower()!="город") continue;
              // if (Dii.Number != 121) continue;
                //StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);      
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
               // if (Dii.Number == 260) res -= 200.5;
                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();
                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();
              
                List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow> res2 = new List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow>();

                PrepTime Pt = new PrepTime()
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name

                };
                List<OrderTimes> Res = TimeOfPreparation.GetOrdersOfDepAndDate(Month, Month.AddMonths(1), Dii.Number, KitchenItems);

                if (Dii.Number != 264)
                {
                    for (DateTime dt = Month; dt < Month.AddMonths(1); dt = dt.AddDays(1))
                    {
                        if (!Res.Any(a =>  a.OrderEndTime > dt && a.OrderEndTime < dt.AddDays(1)))
                        {
                            Console.WriteLine($"Not exist data {Dii.Number} {dt.ToShortDateString()}"); 
                        }
                    }
                }

                foreach (OrderTimes r in Res)
                {
                    Pt.AllCount++;
                    if (r.OrderLastBumpTime > r.ItemCookTime)
                    {
                        Pt.WrongCount++;
                    }

                }

                decimal AllDCount = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                decimal WrongPercent = 0;
                if (Pt.AllCount != 0) {
                    WrongPercent = (decimal)Pt.WrongCount / (decimal)Pt.AllCount;
                };
                decimal GoodCount = AllDCount * (1 - WrongPercent);
                decimal ResbyD = 0;
                if (res != 0)
                {
                    ResbyD = GoodCount / (decimal)res;
                }

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = AllDCount;
                Ws.Cells[row, 3] = WrongPercent;
                Ws.Cells[row, 4] = GoodCount;
                Ws.Cells[row, 5] = res;
                Ws.Cells[row, 6] = ResbyD;
        
                row++;

            }


            //     app.Save(System.Reflection.Missing.Value);

            //      app = null;
        }


        public static void GenRepKitchenTmpYear(DateTime Month)
        {
            //Повар, су-шеф, старший повар
            List<int> KPos = new List<int>() { 2, 8, 4 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            int col = 3;

            Ws.Cells[1, 1] = "Подразделение";
            Ws.Cells[1, 2] = "Кол-во блюд";
            Ws.Cells[1, 3] = "Процент просрочки";
            Ws.Cells[1, 4] = "Кол-во хороших блюд";
            Ws.Cells[1, 5] = "Человеко-часы";
            Ws.Cells[1, 6] = "Блюд в человеко-час";
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(11));
            int row = 2;

            //List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);
            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month, true);

            //   foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            List<int> KitchenItems = CubeData.GetKitchenD().Split(',').ToList().Select(a => Convert.ToInt32(a)).ToList();
            bool c = true;
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim().ToLower() != "город") continue;
                //if (Dii.Number != 205) continue;
                //StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);      
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(12))).TotalHours);
                if (Dii.Number == 260) res -= 200.5*11;
               // int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
               // int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(12))).TotalHours < 6).Count();
              //  List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow> res2 = new List<TimeOfPrep.ShopsGoodTime_T_goodtimeRow>();

                PrepTime Pt = new PrepTime()
                {
                    Dep = Dii.Number,
                    DepName = Dii.Name

                };

                for (int i = 0; i < 12; i++)
                {
                    List<OrderTimes> Res = TimeOfPreparation.GetOrdersOfDepAndDate(Month.AddMonths(i), Month.AddMonths(1+i), Dii.Number, KitchenItems);
                    foreach (OrderTimes r in Res)
                    {
                        Pt.AllCount++;
                        if (r.OrderLastBumpTime > r.ItemCookTime)
                        {
                            Pt.WrongCount++;
                        }

                    }
                }
                decimal AllDCount = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                decimal WrongPercent = 0;
                if (Pt.AllCount != 0)
                {
                    WrongPercent = (decimal)Pt.WrongCount / (decimal)Pt.AllCount;
                };
                decimal GoodCount = AllDCount * (1 - WrongPercent);
                decimal ResbyD = GoodCount / (decimal)res;

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = AllDCount;
                Ws.Cells[row, 3] = WrongPercent;
                Ws.Cells[row, 4] = GoodCount;
                Ws.Cells[row, 5] = res;
                Ws.Cells[row, 6] = ResbyD;

                row++;

            }


            //     app.Save(System.Reflection.Missing.Value);

            //      app = null;
        }

        public static void GenRepKitchenTmpYear2(DateTime Month)
        {
            List<int> KPos = new List<int>() { 2, 8, 4 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            Ws.Name = "Блюда";
            //Worksheet Ws2 = Wb.Worksheets.Add();
            //Ws2.Name = "Монеты";

            //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;

            Ws.Cells[1, 1] = "Подразделение";
            Ws.Cells[1, 2] = "Кол-во блюд";
            Ws.Cells[1, 3] = "Человеко-часы";
            Ws.Cells[1, 4] = "Блюд в человеко-час";

            //Ws2.Cells[1, 1] = "Подразделение";
            //Ws2.Cells[1, 2] = "Кол-во монет";
            //Ws2.Cells[1, 3] = "Человеко-часы";
            //Ws2.Cells[1, 4] = "Монет в человеко-час";

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(11));
            int row = 2;

            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month,true);

            //   foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            foreach (S2010.DepartmentInfo Dii in DepList)
            {
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim().ToLower() != "город") continue;
                //if (Dii.Number != 205) continue;
                //StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);      
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(7))).TotalHours);
                if (Dii.Number == 260) res -= 250.2*11;

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(7))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();
                //List<CEmpl> AllEmpl = StaffBase.GetAllSal(Month, Emps);

                /*
                      foreach (CEmpl emp2 in AllEmpl.OrderBy(a => a.Id))
                      {
                          List<CEmplWt> WtsOfEmpl = Wts.Where(a => a.Dep == Dii.Number && a.Emp.Id == emp2.Id).ToList();
                                                  Ws.Cells[row, 1] = emp2.Id;
                          //Ws.Cells[row, 2] = emp2.Money;
                          Ws.Cells[row, 4] = WtsOfEmpl.Sum(a => GetNightHours(SP, a));
                          Ws.Cells[row, 5] = WtsOfEmpl.Sum(a => (a.StopDt - a.StartDt).TotalHours);
                         // Ws.Cells[row, 4] = emp2.DayHourCount;
                         // Ws.Cells[row, 5] = emp2.NighHourCount;
                          row++;
                      }
                  */


                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;
                Ws.Cells[row, 4] = (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count) / res;


              //  Ws2.Cells[row, 1] = Dii.Name;
              //  Ws2.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.MoneyCount);
              //  Ws2.Cells[row, 3] = res;
              //  Ws2.Cells[row, 4] = (double)Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.MoneyCount) / res;


                //Ws.Cells[row, 3] = CountInDay / 31;
                //Ws.Cells[row, 4] = AllEmpl.Sum(a => a.sal)*30;
                //Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                row++;

            }
        }
        

        public static List<int>  GetPovarOfCeh(int Cn)
        {
            string Path = @"D:\ДН\Повара (1).xlsx";
            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Open(Path);
                      
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Sheets[Cn];
            List<int> res1 = new List<int>();

            for (int row = 1; row < 5000; row++)
            {
                try
                {
                    string v = Ws.Cells[row, 1].Value2.ToString();
                    int r = (int)(double.Parse(v));
                    if ((r>0)&&(!res1.Contains(r)))
                    {
                        res1.Add(r);
                    }
                }
                catch
                { }
            }

            return res1;
            Wb.Close();
            app.Quit();
            

        }

        public static void GenRepStoika2(DateTime Month)
        {
            List<int> KPos = new List<int>() { 5}; //бариста
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            int col = 3;
            Ws.Name = "Бариста";
            Ws.Cells[1, 1] = "Производительность бариста";
            Ws.Cells[2, 1] = "Подразделение";
            Ws.Cells[2, 2] = "Отработанных человеко-часов";
            Ws.Cells[2, 3] = "Приготовленно напитков";
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 3;

            List<DishCount> Dk = CubeData.GetBaristaDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;

                row++;

            }
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Sheets.Add();
            //int col = 3;
            Ws.Name = "Специалист";
            Ws.Cells[1, 1] = "Производительность cпециалиста";
            Ws.Cells[2, 1] = "Подразделение";
            Ws.Cells[2, 2] = "Отработанных человеко-часов";
            Ws.Cells[2, 3] = "Приготовленно напитков";
            KPos = new List<int>() { 6}; //бариста
            Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            row = 3;

            Dk = CubeData.GetSpecDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;

                row++;

            }
        }



        public static void GenRepNapitki(DateTime Month)
        {
            List<int> KPos = new List<int>() { 5,6 }; //
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            int col = 3;
            Ws.Name = "Напитки";
            Ws.Cells[1, 1] = "Производительность бариста";
            Ws.Cells[2, 1] = "Подразделение";
            Ws.Cells[2, 2] = "Отработанных человеко-часов";
            Ws.Cells[2, 3] = "Приготовленно напитков";
            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 3;

            List<DishCount> Dk = CubeData.GetNapitkiDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;

                row++;

            }
            /*
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Sheets.Add();
            //int col = 3;
            Ws.Name = "Специалист";
            Ws.Cells[1, 1] = "Производительность cпециалиста";
            Ws.Cells[2, 1] = "Подразделение";
            Ws.Cells[2, 2] = "Отработанных человеко-часов";
            Ws.Cells[2, 3] = "Приготовленно напитков";
            KPos = new List<int>() { 6 }; //бариста
            Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            row = 3;

            Dk = CubeData.GetSpecDishesCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;

                row++;

            }
             * */
        }


        public static void GenRepPovarcex(DateTime Month)
        {
            List<int> KPos = new List<int>() { 2 }; //бариста
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            int col = 3;
            Ws.Name = "Холодный цех";
            Ws.Cells[1, 1] = "Производительность холодного цеха";
            Ws.Cells[2, 1] = "Подразделение";
            Ws.Cells[2, 2] = "Отработанных человеко-часов";
            Ws.Cells[2, 3] = "Приготовленно блюд";
            List<int> PovarGor = GetPovarOfCeh(2);

            List<CEmplWt> AllWts= StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            List<CEmplWt> Wts = AllWts.Where(a => PovarGor.Contains(a.Emp.Id)).ToList();
            int row = 3;

            List<DishCount> Dk = CubeData.GetHolCexCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;

                row++;

            }
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Sheets.Add();
            //int col = 3;
            Ws.Name = "горячий цех";
            Ws.Cells[1, 1] = "Производительность горячего цеха";
            Ws.Cells[2, 1] = "Подразделение";
            Ws.Cells[2, 2] = "Отработанных человеко-часов";
            Ws.Cells[2, 3] = "Приготовленно блюд";
            
            List<int> PovarHol = GetPovarOfCeh(1);
            Wts = AllWts.Where(a => PovarHol.Contains(a.Emp.Id)).ToList();
            row = 3;

            Dk = CubeData.GetGorCexCount(Month);

            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                int CountInDay = Wts.Where(a => a.Dep == Dii.Number).Count();
                int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = Dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                Ws.Cells[row, 3] = res;

                row++;

            }
        }

        public static void Genrep(DateTime Month)
        {
            List<int> KPos = new List<int> (){2,8};
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //Ws.Cells[1, 2] = "Критерии";

           

            List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col =3;
            //DepList = DepList.OrderBy(a => a.Name).ToList();

           List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row=2;


            List<DishCount> Dk = CubeData.GetKitchenDishesCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
               // if (Dii.Number != 390) continue;
                try
                {

                    StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);
                    double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                    int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 6).Count();
                    int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();


                    List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();


                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = res;
                    Ws.Cells[row, 3] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    Ws.Cells[row, 4] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).Count;

                    // Ws.Cells[row, 4] = AllEmpl.Sum(a=>a.Money);
                    // Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }


            app.Save(System.Reflection.Missing.Value);

            app = null;
        }

        public static void GenrepMoneyPerHour(DateTime Month)
        {
            List<int> KPos = new List<int>();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            //Ws.Cells[1, 2] = "Критерии";



            List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;
            //DepList = DepList.OrderBy(a => a.Name).ToList();

            DateTime StopDt = Month.AddDays(31);

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetAllEmpl(Month, StopDt), Month, StopDt);
            int row = 2;


            List<DishCount> Dk = CubeData.GetAllDishesCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                // if (Dii.Number != 390) continue;
                try
                {

                    //StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);
                    double res = Wts.Where(a => ((a.Dep == Dii.Number) && (a.Emp.Pos != 27) && (a.Emp.Pos != 18) && (a.Emp.Pos != 12))).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, StopDt)).TotalHours);

                    //int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 6).Count();
                    //int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();


               //     List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = res;
                    Ws.Cells[row, 3] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;

                    // Ws.Cells[row, 4] = AllEmpl.Sum(a=>a.Money);
                    // Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }


         
        }


        public static void GenrepStotkaByPeople(DateTime Month)
        {
            List<int> KPos = new List<int>() { 5 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            
            Ws.Cells[1, 1] = "Бариста";
            Ws.Cells[1, 3] = "Отработано, час";
            Ws.Cells[1, 4] = "Сварено чашек";
            Ws.Cells[1, 5] = "Чашек за час";
            Ws.Cells[1, 6] = "Зарплата";
            Ws.Cells[1, 7] = "Ставка";
            Ws.Cells[1, 8] = "Выслуга";


          //  List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;
            //DepList = DepList.OrderBy(a => a.Name).ToList();

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));



            int row = 2;


            List<DishCount> Dk = CubeData.GetBaristaDishesCountWithTime(Month);

            StaffEmpl.StaffEmployeeParametersObjClient cl = new StaffEmpl.StaffEmployeeParametersObjClient();
            Cube2005DataContext cb = new Cube2005DataContext(); 
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                 if ((Dii.Number != 395)) continue;
                try
                {

                   // StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);
                //    double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                    //double res2 = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                 //   int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 6).Count();
                //    int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();


                    List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();



                    Ws.Cells[row, 1] = Dii.Name;
                    row++;

                    List<BaristaPower> BaristasOfDep = new List<BaristaPower>();
                    foreach (int Emp in Emps)
                    {
                        try
                        {


                            BaristaPower Bp = new BaristaPower();
                            Bp.Emp.Id = Emp;
                            Bp.Emp.Name = (from o in cb.StaffEmployees where o.EMPLOYEE_ID == Emp select o.FIRST_NAME + " " + o.LAST_NAME).First();
                            Bp.Emp.EntryDate = StaffBase.GetEntry_date(Emp);
                            BaristasOfDep.Add(Bp);
                        }
                        catch(Exception ee)
                        {
                            Console.WriteLine(ee.Message);
                        }
                    }

                    foreach (DishCount Dcc in Dk.Where(a => a.Dep == Dii.Number))
                    {
                        double BCount = Wts.Where(a => a.Dep == Dii.Number && a.StartDt < Dcc.dt && a.StopDt > Dcc.dt).Count();
                        if (BCount == 0)
                        {
                            continue;
                        }
                        foreach (BaristaPower Bp in BaristasOfDep)
                        {

                            foreach (CEmplWt Wt in Wts.Where(a => a.Dep == Dii.Number && a.Emp.Id == Bp.Emp.Id))
                            {
                                if (Dcc.dt > Wt.StartDt && Dcc.dt < Wt.StopDt)
                                {
                                    Bp.CupCount += (double)Dcc.Count / BCount;
                                    //Bp.Stavka = Wt.
                                  //  Bp.MoneyCount += Dcc.MoneyCount / BCount;
                                }
                            }
                            
                            //if (Wts.Where(a => a.Dep == Dii.Number && a.StartDt < Dcc.dt && a.StartDt > Dcc.dt && Bp.myWts).Count() )
                        }
                    }

                    for (DateTime dt = Month; dt < Month.AddMonths(1); dt = dt.AddDays(1))
                    {
                        StaffEmpl.Emploee_pay_WORKTIME_TEMPRow[] res;
                        try
                        {
                            cl.Emploee_pay(Dii.Number, dt, out res);

                            foreach (StaffEmpl.Emploee_pay_WORKTIME_TEMPRow r in res)
                            {
                                foreach (BaristaPower Bp in BaristasOfDep)
                                {
                                    if (r.emploee_id.Value == Bp.Emp.Id)
                                    {
                                        Bp.FotCount += r.PAYROLL.Value;
                                        Bp.Stavka = r.sallary.Value;
                                    }
                                }
                                
                            }
                        }
                        catch (Exception ee)
                        {
                            Console.WriteLine(ee.Message);
                        }
                    }


                    foreach (BaristaPower Bp in BaristasOfDep)
                    {



                        double resEmpl = Wts.Where(a => a.Dep == Dii.Number && a.Emp.Id==Bp.Emp.Id).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                     
              
                        Ws.Cells[row, 1] = Bp.Emp.Id;
                        Ws.Cells[row, 2] = Bp.Emp.Name;
                        Ws.Cells[row, 3] = resEmpl;
                        Ws.Cells[row, 4] = Bp.CupCount;
                        
                        if (resEmpl>0)
                        {
                            Ws.Cells[row, 5] = (double)Bp.CupCount / resEmpl;
                        }
                        Ws.Cells[row, 6] = Bp.FotCount;
                        Ws.Cells[row, 7] = Bp.Stavka;
                        Ws.Cells[row, 8] = (DateTime.Now - Bp.Emp.EntryDate).TotalDays/365;
                        if ((DateTime.Now - Bp.Emp.EntryDate).TotalDays / 365 > 5)
                        {
                            Ws.Cells[row, 9] = 10;
                        }

                        /*
                        if (Dii.Category == 1)
                        {
                            Ws.Cells[row, 8] = 150;
                        }
                        if (Dii.Category == 2)
                        {
                            Ws.Cells[row, 8] = 120;
                        }
                        if (Dii.Category == 3)
                        {
                            Ws.Cells[row, 8] = 90;
                        }
                         * */
                        row++;      
                    }




                    row++;  
                }

                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Ws.get_Range("C3:C1000").NumberFormat = "0.00";
                Ws.get_Range("D3:D1000").NumberFormat = "0";
                Ws.get_Range("E3:E30").NumberFormat = "0.00";
                Ws.get_Range("F3:F30").NumberFormat = "0 р";
                Ws.get_Range("G3:G30").NumberFormat = "0 р";
            }


         //   app.Save(System.Reflection.Missing.Value);

         //   app = null;
        }


        public static void GenrepStotka2(DateTime Month)
        {

            List<int> KPos = new List<int>() { 12, 5, 6 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = true;
            var staffSrv = new StaffEmpl.StaffEmployeeParametersObjClient();
            staffSrv.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
            int row = 1;
            Ws.Cells[row++, 1] = "Отчет по стойке";
            
            Ws.Cells[row, 1] = "Подразделение";
            Ws.Cells[row, 2] = "Кол-во блюд";
            Ws.Cells[row, 3] = "Человеко-часы";
            Ws.Cells[row++, 4] = "Блюд в человеко-час";


            var dk = CubeData.GetStoikaDishesCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                if (Dii.Place.Trim()!="Город") continue;

                StaffEmpl.working_time_T_wrkRow[] emplres = new StaffEmpl.working_time_T_wrkRow[200];
                var res = staffSrv.working_time(Month, Month.AddMonths(1), Dii.Number, 0, out emplres); 
                var wts = emplres.Where(a => KPos.Contains(a.p_id.GetValueOrDefault()));
                var dcount = dk.Where(a => a.Dep == Dii.Number).Sum(a => a.Count);
                //var pcount = wts.Sum(a=>a.)
                    var pcount = wts.Sum(a => (StaffWtToExcel.GetMinDate(a.dtm_fn.GetValueOrDefault(), Month.AddMonths(1)) - StaffWtToExcel.GetMaxDate(a.dtm_st.GetValueOrDefault(), Month)).TotalHours);
                double dp = Math.Round((double)dcount/(double)pcount,2);

                Ws.Cells[row, 1] = Dii.Name;
                Ws.Cells[row, 2] = dcount;
                Ws.Cells[row, 3] = pcount;
                Ws.Cells[row++, 4] = dp;
                
                
            }
        }


        public static void GenrepStotka(DateTime Month)
        {
            //  List<int> KPos = new List<int>() {  5 };
            List<int> KPos = new List<int>() {12, 5,6 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //Ws.Cells[1, 2] = "Критерии";

            List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;
            //DepList = DepList.OrderBy(a => a.Name).ToList();

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            /*
            var staffSrv = new StaffEmpl.StaffEmployeeParametersObjClient();
            staffSrv.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
            StaffEmpl.working_time_T_wrkRow[] emplres = new StaffEmpl.working_time_T_wrkRow[200];
            var res = staffSrv.working_time(d1, d2, Dii.Number, 0, out emplres); //StaffBase.GetWtsByDep(Dii.Number, Month, Month.AddMonths(1)).Where(a=>a.Emp.Pos!=27);
            var Wts = emplres.Where(a => a.p_id != 27);
            */
            int row = 2;


             //List<DishCount> Dk = CubeData.GetBaristaDishesCount(Month);
            List<DishCount> Dk = CubeData.GetStoikaDishesCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                // if (Dii.Number != 390) continue;
                try
                {

                    StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);
                    double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                    double res2 = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                    int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 6).Count();
                    int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                    List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = res;
                    Ws.Cells[row, 3] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    Ws.Cells[row, 4] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).Count;

                    // Ws.Cells[row, 4] = AllEmpl.Sum(a=>a.Money);
                    // Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }


            app.Save(System.Reflection.Missing.Value);

            app = null;
        }

        public static void GenrepStotkaSpecOnly(DateTime Month)
        {
            //  List<int> KPos = new List<int>() {  5 };
            List<int> KPos = new List<int>() { 6 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //Ws.Cells[1, 2] = "Критерии";

            List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;
            //DepList = DepList.OrderBy(a => a.Name).ToList();

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 2;


            //List<DishCount> Dk = CubeData.GetBaristaDishesCount(Month);
            List<DishCount> Dk = CubeData.GetSpecDishesCount(Month);
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                // if (Dii.Number != 390) continue;
                try
                {

                    StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);
                    double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);
                    double res2 = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                    int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 6).Count();
                    int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();

                    List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();

                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = res;
                    Ws.Cells[row, 3] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;
                    Ws.Cells[row, 4] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).Count;

                    // Ws.Cells[row, 4] = AllEmpl.Sum(a=>a.Money);
                    // Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }


            app.Save(System.Reflection.Missing.Value);

            app = null;
        }
        public static void GenrepStotkaWithoutKofe(DateTime Month)
        {
            List<int> KPos = new List<int>() { 6 };
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook Wb = app.Workbooks.Add(true);
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            //Ws.Cells[1, 2] = "Критерии";
            app.Visible = true;


            List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
            int col = 3;
            //DepList = DepList.OrderBy(a => a.Name).ToList();

            List<CEmplWt> Wts = StaffBase.GetWts(StaffBase.GetEmplsOfPos(Month, KPos), Month, Month.AddMonths(1));
            int row = 2;


            List<DishCount> Dk = CubeData.GetStoikaDishesCountWithoutCoffee(Month);




            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            {
                if (!Dii.Enabled) continue;
                // if (Dii.Number != 390) continue;
                try
                {

                    StaffParams SP = SParams.FirstOrDefault(a => a.DepNum == Dii.Number);
                    double res = Wts.Where(a => a.Dep == Dii.Number).Sum(a => (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours);

                    int CountInDay = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours > 6).Count();
                    int CountInDayMin = Wts.Where(a => a.Dep == Dii.Number && (GetMaxDate(a.StopDt, Month) - GetMinDate(a.StartDt, Month.AddMonths(1))).TotalHours < 6).Count();


                    List<int> Emps = Wts.Where(a => a.Dep == Dii.Number).Select(a => a.Emp.Id).Distinct().ToList();




                    Ws.Cells[row, 1] = Dii.Name;
                    Ws.Cells[row, 2] = res;
                    Ws.Cells[row, 3] = Dk.FirstOrDefault(a => a.Dep == Dii.Number).MoneyCount;

                    // Ws.Cells[row, 4] = AllEmpl.Sum(a=>a.Money);
                    // Ws.Cells[row, 5] = AllEmpl.Average(a => a.sal); ;


                    row++;
                }
                catch
                { }
            }


          
        }

        private static double GetNightHours(StaffParams Param, CEmplWt Wt)
        {
            if ((Wt.StopDt - Wt.StartDt).TotalHours > 24)
            {
                return 0;
            }
            
            DateTime StartNightOfStart = new DateTime(Wt.StartDt.Year,Wt.StartDt.Month,Wt.StartDt.Day,Param.StartNight.Hours,Param.StartNight.Minutes,0);
            DateTime StartNightOfEnd = new DateTime  (Wt.StopDt.Year,Wt.StopDt.Month,Wt.StopDt.Day,Param.StartNight.Hours,Param.StartNight.Minutes,0);
            DateTime StopNightOfEnd = new DateTime  (Wt.StopDt.Year,Wt.StopDt.Month,Wt.StopDt.Day,Param.StopNight.Hours,Param.StopNight.Minutes,0);
            DateTime StopNightOfStart = new DateTime  (Wt.StartDt.Year,Wt.StartDt.Month,Wt.StartDt.Day,Param.StopNight.Hours,Param.StopNight.Minutes,0);
            if (Wt.StartDt > StartNightOfStart) 
            {
                if ((Wt.StartDt.Date) == (Wt.StopDt.Date))
                {
                    return (Wt.StopDt - Wt.StartDt).TotalHours;
                }
                else 
                {
                    if (Wt.StopDt.Hour < Param.StartNight.Hours)
                    {
                        //DateTime StartNightOfEnd = new DateTime  (Wt.StopDt.Year,Wt.StopDt.Month,Wt.StopDt.Day,Param.StartNight.Hours,Param.StartNight.Minutes,0);
                        return (GetMinDate(StopNightOfEnd, Wt.StopDt) - Wt.StartDt).TotalHours;
                    }
                    else
                    {
                        return (GetMinDate(StopNightOfEnd, Wt.StopDt) - Wt.StartDt).TotalHours + (Wt.StopDt-StartNightOfEnd).TotalHours;
                    }
                }
            }
            else if (Wt.StartDt < StopNightOfStart)
            {
                if ((Wt.StartDt.Date) == (Wt.StopDt.Date))
                {
                    if (Wt.StopDt.Hour < Param.StartNight.Hours)
                    {
                        //DateTime StartNightOfEnd = new DateTime  (Wt.StopDt.Year,Wt.StopDt.Month,Wt.StopDt.Day,Param.StartNight.Hours,Param.StartNight.Minutes,0);
                        return (GetMinDate(StopNightOfEnd, Wt.StopDt) - Wt.StartDt).TotalHours;
                    }
                    else
                    {
                        return (GetMinDate(StopNightOfEnd, Wt.StopDt) - Wt.StartDt).TotalHours + (Wt.StopDt - StartNightOfEnd).TotalHours;
                    }
                }
                else
                {
                    return (StopNightOfStart - Wt.StartDt).TotalHours + (Wt.StopDt - StartNightOfStart).TotalHours;
                
                }
            }
            else if (Wt.StartDt < StartNightOfStart)
            {
                if ((Wt.StartDt.Date) == (Wt.StopDt.Date))
                {
                    if (Wt.StopDt < StartNightOfStart)
                    {
                        return 0;
                    }
                    else
                    {
                        return (GetMinDate(StopNightOfEnd, Wt.StopDt) - StartNightOfStart).TotalHours;
                    }
                }
                else
                {
                    return (GetMinDate(StopNightOfEnd, Wt.StopDt) - StartNightOfStart).TotalHours;
                }
            
            }
            return 0;

        
        
        }


        public static DateTime GetMaxDate(DateTime dt1, DateTime dt2)
        {
            if (dt1.CompareTo(dt2) == 1)
            {
                return dt1;
            }
           else
            {
                return dt2;
            }

            
        
        }
        public static DateTime GetMinDate(DateTime dt1, DateTime dt2)
       {
           if (dt1.CompareTo(dt2) == 1)
           {
               return dt2;
           }
           else
           {
               return dt1;
           }



       }
    }
}
