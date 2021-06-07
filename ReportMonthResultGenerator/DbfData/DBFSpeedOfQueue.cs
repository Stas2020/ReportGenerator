using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace ReportMonthResultGenerator.DbfData
{
   
   
        public class DBFSpeedOfQueue
        {
            //string folderPath;
            bool CanWork = false;

            public DBFSpeedOfQueue(string fullNumFolderPath = "")
            {
                if (fullNumFolderPath != null)
                {
                    if (Directory.Exists(fullNumFolderPath))
                    {
                        CanWork = true;
                    }
                }

            }
            public bool Parse(string folderPath)
            {
                

                if (CanWork)
                {

                    using (var db = new CheckLongTestDbDataContext() )
                    {
                        DirectoryInfo numFolder = new DirectoryInfo(folderPath);

                        string folder = numFolder.FullName;
                        string table = "TABLEID";
                        string tableCheck = "TABLEID,CHECK";
                        string cmdString = table;

                        if (File.Exists(folder + @"\GNDTURN.DBF"))
                        {
                            DbfReader dbfTest = new DbfReader(folder);

                            try
                            {
                                var dtTest = dbfTest.Execute(@"select TOP 1 CHECK from " + folder + "\\GNDTURN.DBF");

                                foreach (DataRow dRow in dtTest.Rows)
                                {
                                    cmdString = tableCheck;
                                }

                            }
                            catch
                            {
                                cmdString = table;
                            }

                            dbfTest = null;
                        }

                        if (File.Exists(folder + @"\GNDITEM.DBF"))
                        {
                            DbfReader dbfI = new DbfReader(folder);
                            try
                            {
                                var dtI = dbfI.Execute(@"select EMPLOYEE,CHECK,ITEM,PARENT,DOB,SYSDATE,ETIME_HOUR,ETIME_MIN,ETIME_SEC,UNIT,TABLEID from " + folder + "\\GNDITEM.DBF");

                                foreach (DataRow dRow in dtI.Rows)
                                {
                                Items nItem = new Items();

                                    /// селект мой - можно по индексам обращаться

                                    nItem.Employee = int.Parse(dRow[0].ToString());
                                    nItem.CheckNum = int.Parse(dRow[1].ToString());
                                    nItem.ItemNum = int.Parse(dRow[2].ToString());
                                    nItem.ParentNum = int.Parse(dRow[3].ToString());
                                    nItem.DepNum = int.Parse(dRow[9].ToString());
                                    nItem.TableId = int.Parse(dRow[10].ToString());

                                    nItem.DOB = DateTime.Parse(dRow[4].ToString()).Date;

                                    DateTime sysTime = DateTime.Parse(dRow[5].ToString());

                                    nItem.SysDate = sysTime.Date;

                                    DateTime startTime = new DateTime(sysTime.Year, sysTime.Month, sysTime.Day, int.Parse(dRow[6].ToString()), int.Parse(dRow[7].ToString()), int.Parse(dRow[8].ToString()));  // год - месяц - день - час - минута - секунда
                                    nItem.StartTime = startTime;

                                    db.Items.InsertOnSubmit(nItem);



                                }
                            }
                            catch
                            {
                                Console.WriteLine("Ошибка - " + folder + @"\GNDITEM.DBF");
                            }

                            dbfI = null;
                        }
                        else
                        {
                            Console.WriteLine("Файл отсутствует " + folder + @"\GNDITEM.DBF");
                        }

                        if (File.Exists(folder + @"\GNDTURN.DBF"))
                        {
                            DbfReader dbfT = new DbfReader(folder);
                            try
                            {
                                var dtT = dbfT.Execute(@"select EMPLOYEE,UNIT,DOB,OPENHOUR,OPENMIN,OPENSEC,CLOSEHOUR,CLOSEMIN,CLOSESEC," + cmdString + " from " + folder + "\\GNDTURN.DBF");

                                foreach (DataRow dRow in dtT.Rows)
                                {
                                    Turns nTurn = new Turns();

                                    nTurn.Employee = int.Parse(dRow[0].ToString());

                                    nTurn.DepNum = int.Parse(dRow[1].ToString());

                                    DateTime dob = DateTime.Parse(dRow[2].ToString());
                                    nTurn.DOB = dob.Date;

                                    DateTime openTime = new DateTime(dob.Year, dob.Month, dob.Day, int.Parse(dRow[3].ToString()), int.Parse(dRow[4].ToString()), int.Parse(dRow[5].ToString()));
                                    nTurn.OpenTime = openTime;

                                    DateTime closeTime = new DateTime(dob.Year, dob.Month, dob.Day, int.Parse(dRow[6].ToString()), int.Parse(dRow[7].ToString()), int.Parse(dRow[8].ToString()));

                                    if (closeTime < openTime)
                                    {
                                        closeTime.AddDays(1);
                                    }
                                    nTurn.CloseTime = closeTime;

                                    nTurn.TableId = int.Parse(dRow[9].ToString());

                                    if (cmdString == tableCheck)
                                    {
                                        nTurn.CheckNum = int.Parse(dRow[10].ToString());
                                    }

                                    db.Turns.InsertOnSubmit(nTurn);

                                }

                            }
                            catch
                            {
                                Console.WriteLine("Ошибка - " + folder + @"\GNDTURN.DBF");
                            }

                            dbfT = null;
                        }
                        else
                        {
                            Console.WriteLine("Файл отсутствует " + folder + @"\GNDTURN.DBF");
                        }

                        if (File.Exists(folder + @"\GNDTNDR.DBF"))
                        {
                            DbfReader dbfTDr = new DbfReader(folder);

                            try
                            {
                                var dtTDr = dbfTDr.Execute(@"select EMPLOYEE,CHECK,STRUNIT,DATE,SYSDATE,TYPEID,AMOUNT from " + folder + "\\GNDTNDR.DBF");

                                foreach (DataRow dRow in dtTDr.Rows)
                                {

                                    TurnsDr nTurnDr = new TurnsDr();

                                    nTurnDr.Employee = int.Parse(dRow[0].ToString());
                                    nTurnDr.CheckNum = int.Parse(dRow[1].ToString());
                                    nTurnDr.DepNum = int.Parse(dRow[2].ToString());
                                    nTurnDr.TypeId = int.Parse(dRow[5].ToString());
                                    nTurnDr.Amount = double.Parse(dRow[6].ToString());

                                    nTurnDr.DOB = DateTime.Parse(dRow[3].ToString());
                                    nTurnDr.SysDate = DateTime.Parse(dRow[4].ToString());

                                    db.TurnsDr.InsertOnSubmit(nTurnDr);

                                    

                                }
                            }
                            catch
                            {
                                Console.WriteLine("Ошибка - " + folder + @"\GNDTNDR.DBF");
                            }

                            dbfTDr = null;
                        }
                        else
                        {
                            Console.WriteLine("Файл отсутствует " + folder + @"\GNDTNDR.DBF");
                        }

                         db.SubmitChanges();
                    return true;
                    }

                    


                }
                else
                {
                    return false;
                }

            }

            public class DbfReader
            {
                private OleDbConnection _connection = null;

                public DataTable Execute(string command)
                {
                    DataTable dt = null;
                    if (_connection != null)
                    {
                        try
                        {
                            _connection.Open();
                            dt = new DataTable();
                            System.Data.OleDb.OleDbCommand oCmd = _connection.CreateCommand();
                            oCmd.CommandText = command;
                            dt.Load(oCmd.ExecuteReader());
                            _connection.Close();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    return dt;
                }

                public DataTable GetAll(string dbpath)
                {
                    return Execute("SELECT * FROM " + dbpath);
                }

                public DbfReader(string Folder)
                {
                    this._connection = new System.Data.OleDb.OleDbConnection();
                    _connection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Folder + "; Extended Properties=dBASE IV;";
                }
            }
        }

    }

