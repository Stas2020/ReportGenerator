using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Data.Odbc;

namespace ReportMonthResultGenerator.DbfData
{
    public static class DBFExtractor
    {

        static System.Data.Odbc.OdbcConnection dbfConnection;
        static public void ConnOpen(string Path)
        {
            dbfConnection = new OdbcConnection();

            string strConString = @"Driver={Microsoft dBASE Driver (*.dbf)};DriverID=277;Dbq=" + Path + ";";
            try
            {
                dbfConnection = new OdbcConnection();
                dbfConnection.ConnectionString = strConString;
                dbfConnection.Open();
            }
            catch
            {
                dbfConnection = new OdbcConnection();
                dbfConnection.ConnectionString = @"Driver={Microsoft Access dBASE Driver (*.dbf, *.ndx, *.mdx)};Dbq=" + Path + ";";
                dbfConnection.Open();
            }

        }

        public static void GetAvgTimeOfTableNY()
        {
            var path = @"D:\NYRep\2019";
            var di = new DirectoryInfo(path);
            double SummTime = 0;
            long AllCount = 0;
            foreach (var dii in di.GetDirectories())
            {
                ConnOpen(dii.FullName);
                string CommandStr = "Select  openhour, openmin , opensec, closehour, closemin, closesec from GNDTURN";
                OdbcCommand Command = new OdbcCommand(CommandStr, dbfConnection);
                OdbcDataReader or = Command.ExecuteReader();
                while (or.Read())
                {
                    try
                    {

                        DateTime StartDT = new DateTime(2018, 1, 1, or.GetInt32(0), or.GetInt32(1), or.GetInt32(2));
                        DateTime EndDT = new DateTime(2018, 1, 1, or.GetInt32(3), or.GetInt32(4), or.GetInt32(5));
                        var d = (EndDT - StartDT).TotalSeconds;
                        if ((d > 5*60)&&(d<60*120))
                        {
                            SummTime += d;
                            AllCount++;
                        }

                    }
                    catch (Exception e)
                    {
                        Utils.ToLog("Error GNDITEM.dbf " + dii.FullName + " Err: " + e.Message);
                    }
                }
                or.Close();
                Command.Dispose();
                dbfConnection.Close();
                Console.WriteLine($"dii: {dii.Name};  SummTime: {SummTime}; AllCount: {AllCount}; d: {SummTime / AllCount}");
            }
            Console.WriteLine($"SummTime: {SummTime}; AllCount: {AllCount}; d: {SummTime / AllCount}");
            Console.Read();
        }

        public static void ReportGen()
        {
            List<int> Vines = CubeData.GetVineD();
            ReportBaseDataContext RbDC = new ReportBaseDataContext();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            foreach (S2010.DepartmentInfo DI in DepList.Where(a => a.Enabled && a.Place.Trim().ToLower() == "город"))
            //foreach (S2010.DepartmentInfo DI in DepList.Where(a => a.Enabled))
            {
                var Itms = from o in RbDC.ReportDBFDishes where o.Dep == DI.Number && Vines.Contains(o.BarCode.Value) select o;
                //Dictionary<DateTime, int> Cont = new Dictionary<DateTime, int>();
                List<Tuple<DateTime, int>> Cont = new List<Tuple<DateTime, int>>();
                int res = 0;
                foreach (var itm in Itms)
                {
                    if (Cont.Where(a => a.Item1 == itm.BusinessDate && a.Item2 == itm.CheckId).Count() > 0)
                    {
                        continue;
                    }
                    res += Itms.Where(a => a.BusinessDate == itm.BusinessDate && a.CheckId == itm.CheckId && a.OrderTime > itm.OrderTime.Value.AddMinutes(60) ).Count();
                    Cont.Add(Tuple.Create(itm.BusinessDate.Value, itm.CheckId.Value));
                }
                Utils.ToLog(String.Format("{0}; {1}; {2}", DI.Name, Itms.Count(), res));
            }

        }

            static string DBFsPath = @"\\cube2005\g$\ArhivFilesDownload\data\DBF\";
        //static String TmpFoder = "Tmp";
        public static void ExtractDBFs(DateTime StartDT)
        {
            ReportBaseDataContext RbDC = new ReportBaseDataContext();
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();
            foreach (S2010.DepartmentInfo DI in DepList.Where(a => a.Enabled && a.Place.Trim().ToLower() == "город"))
            //foreach (S2010.DepartmentInfo DI in DepList.Where(a => a.Enabled))
            {
                DirectoryInfo DBFDir = new DirectoryInfo(DBFsPath + DI.AlohaStr + "1");
                if (!DBFDir.Exists)
                {
                    Utils.ToLog("DBF dep not exist " + DI.AlohaStr);
                    continue;
                }

                for (DateTime dt = StartDT; dt < DateTime.Now.AddDays(-1); dt = dt.AddDays(1))
                {
                    FileInfo fi = new FileInfo(DBFDir.FullName + @"\" + dt.ToString("yyyyMMdd") + ".rar");
                    if ((from o in RbDC.ReportDBFFilesComplited where (o.Dep == DI.AlohaStr && o.FileName == fi.Name)  select o ).Count() > 0)
                    {
                        
                        Utils.ToLog("File already parced" + fi.FullName);
                        continue;
                    }
                    try
                    {
                        if (!fi.Exists)
                        {
                            Utils.ToLog("File not exist " + fi.FullName);
                            continue;
                        }

                        DirectoryInfo TmpFolderPath = new DirectoryInfo("Tmp");
                        if (!TmpFolderPath.Exists)
                        {
                            TmpFolderPath.Create();
                        }
                        string NewFPath = TmpFolderPath.FullName + @"\" + fi.Name;
                        //string NewDirPath = TmpFolderPath.FullName + @"\" + fi.Name;
                        fi.CopyTo(NewFPath, true);
                        //Directory.CreateDirectory(TmpFolderPath.FullName + @"\" + Path.GetFileNameWithoutExtension(fi.Name));

                        string UnRarDirPath = TmpFolderPath.FullName + @"\" + Path.GetFileNameWithoutExtension(fi.Name) + @"\";
                        string RarPath = @"C:\Rshd\rar.exe";
                        //string RarComand =  @"e -u "+NewFPath +" "+ UnRarDirPath;
                        string RarComand = @"x -o+ " + NewFPath + " " + UnRarDirPath;
                        System.Diagnostics.Process.Start(RarPath, RarComand);

                        Thread.Sleep(2000);

                        ConnOpen(UnRarDirPath);

                        string Tmp = "";
                        if (!File.Exists(UnRarDirPath + @"GNDITEM.dbf"))
                        {
                            Utils.ToLog("GNDITEM.dbf not exist " + UnRarDirPath);
                            continue;
                        }

                        string CommandStr = "Select EMPLOYEE, check, item, hour,minute,dob, entryid,unit,sysdate from GNDITEM";
                        OdbcCommand Command = new OdbcCommand(CommandStr, dbfConnection);
                        OdbcDataReader or = Command.ExecuteReader();
                        while (or.Read())
                        {
                            try
                            {
                                ReportDBFDishes RepDb = new ReportDBFDishes()
                                {
                                    Empl = or.GetInt32(0),
                                    CheckId = or.GetInt32(1),
                                    BarCode = or.GetInt32(2),
                                    Dep = or.GetInt32(7),
                                    EntryId = (int)or.GetDouble(6),
                                    OrderTime = or.GetDate(8).AddHours(or.GetDouble(3)).AddMinutes(or.GetDouble(4)),
                                    BusinessDate = or.GetDate(5)
                                };

                              //  if ((from o in RbDC.ReportDBFDishes where (o.BusinessDate == RepDb.BusinessDate && o.EntryId == RepDb.EntryId && o.CheckId == RepDb.CheckId && o.Dep == RepDb.Dep) select o ).Count() == 0)
                                {
                                    RbDC.ReportDBFDishes.InsertOnSubmit(RepDb);
                                }
                            }
                            catch (Exception e)
                            {
                                Utils.ToLog("Error GNDITEM.dbf " + UnRarDirPath + " Err: " + e.Message);
                            }
                        }
                        or.Close();
                        Command.Dispose();
                        RbDC.SubmitChanges();
                        Utils.ToLog("Sucsess " + fi.FullName);

                        ReportDBFFilesComplited DbfCompl = new ReportDBFFilesComplited()
                        {
                            Dep = DI.AlohaStr,
                            FileName = fi.Name
                        };
                        RbDC.ReportDBFFilesComplited.InsertOnSubmit(DbfCompl);
                        RbDC.SubmitChanges();

                    }
                    catch (Exception ee)
                    {
                        Utils.ToLog("Error fi " + fi.FullName + " mess: " + ee.Message);
                    }

                    //ZipFile.ExtractToDirectory(NewFPath, TmpFolderPath.FullName + @"\" + Path.GetFileNameWithoutExtension(fi.Name));

                }

            }
        }
    }
}
