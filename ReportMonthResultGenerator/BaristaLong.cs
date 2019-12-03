using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace ReportMonthResultGenerator
{


    
  static  class BaristaLong
    {

      public static void ExcelBaristaGen()
      {
          Application app = new Microsoft.Office.Interop.Excel.Application();
          Workbook Wb = app.Workbooks.Add(true);
          Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

          List<EmpHist> mBarista = Barista();
          int row =2;
          Ws.Cells[1, 1] = "Номер";
          Ws.Cells[1, 2] = "Дата приема на должность бариста";
          Ws.Cells[1, 3] = "Прошлая дожность";
          Ws.Cells[1, 4] = "Дата прошлой дожности";

          foreach(EmpHist Eh in mBarista)
          {
          Ws.Cells[row, 1] = Eh.EmpId;
          Ws.Cells[row, 2] = Eh.BaristaStart.ToString("dd.MM.yyyy");
          Ws.Cells[row, 3] = Eh.LastPosition;
          Ws.Cells[row, 4] = Eh.BaristaStartLerning.ToString("dd.MM.yyyy"); ;
          row++;
          }
          Ws.get_Range("B2:B500").NumberFormat = "ДД.ММ.ГГГГ"; ;
          Ws.get_Range("D2:D500").NumberFormat = "ДД.ММ.ГГГГ"; ;


          //Ws.Cells[1, 2] = "Критерии";

          app.Save(System.Reflection.Missing.Value);

          app = null;

      }

      static DateTime dt = new DateTime(2015, 3, 1);
      public static StaffEmpl.procedure_for_history_t_emphistoryRow GetLastNonBarista(StaffEmpl.procedure_for_history_t_emphistoryRow bar,StaffEmpl.procedure_for_history_t_emphistoryRow[] All)
      {
          try
          {
              return All.ToList().Where(a => ((a.position_id != 5) && (a.work_from_date < dt))).OrderBy(a => a.work_from_date).Last();
          }
          catch
          {

              return null;
          }
      
      }
        public static List<EmpHist> Barista()
        {
            List<EmpHist> Tmp = new List<EmpHist>();
            int Count=0;
     
        StaffEmpl.StaffEmployeeParametersObjClient cl = new StaffEmpl.StaffEmployeeParametersObjClient();
        for(int i =2000;i<12000;i++)
        {
            try
            {
                StaffEmpl.procedure_for_history_t_emphistoryRow[] hist = new StaffEmpl.procedure_for_history_t_emphistoryRow[10];
                cl.procedure_for_history(i, out hist);

                if ((hist == null) || (hist.Length == 0)) continue;
                List<StaffEmpl.procedure_for_history_t_emphistoryRow> LHist = hist.ToList().OrderBy(a => a.work_from_date).ToList();
                if (LHist.Last().work_from_date.Value < dt.AddYears(-1)) continue;
                foreach (StaffEmpl.procedure_for_history_t_emphistoryRow hOne in LHist)
                {
                    if (hOne.position_id == 5)
                    {

                        try
                        {
                            int LastPos = LHist.Where(a => (a.work_from_date < hOne.work_from_date)).OrderBy(a => a.work_from_date).Last().position_id.Value;
                            if (LastPos == 5) continue;
                        }
                        catch
                        {

                        }

                        if (hOne.work_from_date > dt.AddYears(-1))
                        {
                            Count++;
                            EmpHist eh = new EmpHist()
                            {
                                BaristaStart = hOne.work_from_date.Value,
                                EmpId = hOne.employee_id.Value,
                                EmpName = "",
                                LastPosition = ""
                            };
                            StaffEmpl.procedure_for_history_t_emphistoryRow Before = GetLastNonBarista(hOne, hist);
                            if (Before == null)
                            {

                            }
                            else
                            {
                                eh.BaristaStartLerning = Before.work_from_date.Value;
                                eh.LastPosition = Encoding.UTF8.GetString(
                                 Encoding.GetEncoding(1251).GetBytes(Before.position_name));
                            }
                            Tmp.Add(eh);
                        }
                    }
                }
            }
            catch
            {
                cl = new StaffEmpl.StaffEmployeeParametersObjClient();
            }
        }
        //Console.WriteLine(count.to);
        return Tmp;
        }
    }
  public class EmpHist
  {
      public EmpHist()
      { }
      public int EmpId;
          public string EmpName;
          public DateTime BaristaStart;
          public DateTime BaristaStartLerning;
          public string LastPosition;
         public double BaristaLong
          {
              get
              {
                  return (BaristaStart - BaristaStartLerning).TotalDays;
              
              }
          }
  }
}
