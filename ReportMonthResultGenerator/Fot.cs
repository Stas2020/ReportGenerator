using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
    class Fot
    {
        public static void GetFot(DateTime Month, int Department)
        {


            StaffEmpl.StaffEmployeeParametersObjClient Cl = new StaffEmpl.StaffEmployeeParametersObjClient();
            
            List<StaffEmpl.division_pay_EMPLOEE_LISTRow> Pos = new List<StaffEmpl.division_pay_EMPLOEE_LISTRow>();

            Pos.Add(new StaffEmpl.division_pay_EMPLOEE_LISTRow()
                        {
                            emploee_id = 2118
                        }
                    );
            


            StaffEmpl.division_pay_EMPLOEE_PARAMRow[] emppar = new StaffEmpl.division_pay_EMPLOEE_PARAMRow[50];
            decimal? res = 0;
            Cl.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);

            string s = Cl.division_pay(Department, Month, Pos.ToArray(), out res, out emppar);
            Console.WriteLine(s);
            Console.WriteLine(res);
            Console.ReadKey();



        }
    }
}
