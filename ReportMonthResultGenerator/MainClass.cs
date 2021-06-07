using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportMonthResultGenerator
{
   public static class MainClass
    {
       public static void UpdateDesertsOnStopTime(DateTime dt)
       {
           ReportTableUpdater.DesertOnStopUpdate (DesertsOnStop.GetDesertsOnStop(dt, dt.AddMonths(1)), dt);

       }
       public static void UpdateDissmissPercent(DateTime dt)
       { 
         ReportTableUpdater.DismPercentUpdate(DismissalPercent.GetStaffDismissal(dt, dt.AddMonths(1)),dt);
        
       }
       public static void UpdateOrderTimePercent(DateTime dt)
       {
            //ReportTableUpdater.OrderTimeUpdate (TimeOfPreparation.GetTimeOfPrep(dt, dt.AddMonths(1)), dt);
            ReportTableUpdater.OrderTimeUpdate(TimeOfPreparation.GetTimeOfPrepOrder(dt, dt.AddMonths(1)), dt);

        }
        public static void UpdateOrderTimeWODeliveryPercent(DateTime dt)
        {
            //ReportTableUpdater.OrderTimeUpdate (TimeOfPreparation.GetTimeOfPrep(dt, dt.AddMonths(1)), dt);
            ReportTableUpdater.OrderTimeWODeliveryUpdate(TimeOfPreparation.GetTimeOfPrepOrder(dt, dt.AddMonths(1), true), dt);

        }
        public static void UpdateRashMatPercent(DateTime dt)
       {
           ReportTableUpdater.RashMatUpdate ((сRashMat.GetRashMat(dt, dt.AddMonths(1))),dt);

       }
       public static void UpdateDesertSpisaniePercent(DateTime dt)
       {
           ReportTableUpdater.DesertSpisanieUpdate(( Spisanie.GetDesertsSpis(dt, dt.AddMonths(1))), dt);

       }
       public static void UpdateDesertonChk(DateTime dt)
       {
           ReportTableUpdater.DesertOnCheckUpdate(DesertsOnChk.GetDesertsOnChk(dt), dt);
           

       }

       public static void GenSousReport(DateTime dt)
       {
           //           ReportTableUpdater.OrderTimeUpdate(TimeOfPreparation.GetTimeOfPrep(dt, dt.AddMonths(1)), dt);
           Spisanie.SpisListToExcel(Spisanie.GetSousSpis(dt, dt.AddMonths(6)));
           

       }
       
    }
}
