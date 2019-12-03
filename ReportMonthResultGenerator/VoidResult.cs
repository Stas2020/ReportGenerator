using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;


namespace ReportMonthResultGenerator
{
    class VoidResult
    {
        public void GetRes()
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            int i = 1;
            Worksheet Ws;


            List<int> deps = new List<int>() { 211, 210, 209 };
            List<int> K = new List<int> { 2, 4, 20,24,30,34 };
            List<int> W = new List<int> { 15,27,3};
            List<int> St = new List<int> { 7,6,5 };
            List<int> PAll = new List<int>();
            for (int P = 0; P < 200; P++)
            {
                PAll.Add(P);  
            }
            PAll = PAll.Except(K).Except(W).Except(St).ToList();
            List <string> DNames = new  List<string>{"Кухня", "Официанты", "Стойка", "Остальные"};
            List<List<int>> Dpos = new List<List<int>> { K, W, St, PAll };
            foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
           // foreach (int dep in deps)
            {
                if (!Dii.Enabled) continue;
                if ((Dii.Number!=390)) continue;
                for (int Doljn = 0; Doljn < 4; Doljn++)
                {
                    string DName = "";
                   
                    Ws = Wb.Worksheets.Add();
                    Ws.Name = Dii.Name+" " +DNames[Doljn];


                    //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
                    int col = 3;


                    Ws.Columns[2, Type.Missing].EntireColumn.ColumnWidth = 80;


                    Ws.Cells[1, 1] = "№";
                    Ws.Cells[1, 2] = "Вопрос";
                    Ws.Cells[1, 3] = "Всего ответило";
                    Ws.Cells[1, 4] = "1";
                    Ws.Cells[1, 5] = "2";
                    Ws.Cells[1, 6] = "3";
                    Ws.Cells[1, 7] = "4";
                    Ws.Cells[1, 8] = "5";
                    i = 1;
                    VoitingDataContext db = new VoitingDataContext();
                    List<QuestQuestion> QQ = (from o in db.QuestQuestions select o).ToList();
                    Ws.Range["D2:H13"].NumberFormat = "0.0";

                    foreach (QuestQuestion Q in QQ)
                    {
                        Ws.Cells[i + 1, 1] = i.ToString();
                        Ws.Cells[i + 1, 2] = Q.Text;


                        int AllAnswers = (from o in db.QuestAnswers where o.EmpDepNum == Dii.Number && o.QuestionId == Q.Id && Dpos[Doljn].Contains(o.EmpDepPos.Value) select o).Count();
                        Ws.Cells[i + 1, 3] = AllAnswers.ToString();
                        for (int j = 1; j < 6; j++)
                        {
                            int Answ = (from o in db.QuestAnswers where o.EmpDepNum == Dii.Number && o.QuestionId == Q.Id && Dpos[Doljn].Contains(o.EmpDepPos.Value) && o.Result == j select o).Count();
                            Ws.Cells[i + 1, j + 3] = ((double)Answ * 100 / (double)AllAnswers);
                        }

                        i++;
                    }
                }
            }

            Ws = Wb.Worksheets.Add();
            Ws.Name = "Все";

             i = 1;
            VoitingDataContext db2 = new VoitingDataContext();
            List<QuestQuestion> QQ2 = (from o in db2.QuestQuestions select o).ToList();
            Ws.Range["D2:H13"].NumberFormat = "0.0";
            foreach (QuestQuestion Q in QQ2)
            {
                Ws.Cells[i + 1, 1] = i.ToString();
                Ws.Cells[i + 1, 2] = Q.Text;


                int AllAnswers = (from o in db2.QuestAnswers where o.QuestionId == Q.Id select o).Count();
                Ws.Cells[i + 1, 3] = AllAnswers.ToString();
                for (int j = 1; j < 6; j++)
                {
                    int Answ = (from o in db2.QuestAnswers where o.QuestionId == Q.Id && o.Result == j select o).Count();
                    Ws.Cells[i + 1, j + 3] = ((double)Answ * 100 / (double)AllAnswers);
                }

                i++;
            }


        }

          public void GetRes2()
        {
            S2010.XrepSoapClient Serv = new S2010.XrepSoapClient();
            S2010.DepartmentInfo[] DepList = Serv.GetPointList3();

            Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Workbook Wb = app.Workbooks.Add(true);
            int i = 1;
            Worksheet Ws;


            List<int> deps = new List<int>() { 210, 211, 214 };
            
            
         //   foreach (S2010.DepartmentInfo Dii in DepList.OrderBy(a => a.Name))
            foreach (int dep in deps)
            {
              
                
                    string DName = "";
                   
                    Ws = Wb.Worksheets.Add();
                    Ws.Name = dep.ToString();


                    //List<StaffParams> SParams = StaffBase.GetParametrsOfStaff(Month);
                    int col = 3;


                    Ws.Columns[2, Type.Missing].EntireColumn.ColumnWidth = 80;


                    Ws.Cells[1, 1] = "№";
                    Ws.Cells[1, 2] = "Вопрос";
                    Ws.Cells[1, 3] = "Всего ответило";
                    Ws.Cells[1, 4] = "1";
                    Ws.Cells[1, 5] = "2";
                    Ws.Cells[1, 6] = "3";
                    Ws.Cells[1, 7] = "4";
                    Ws.Cells[1, 8] = "5";
                    i = 1;
                    VoitingDataContext db = new VoitingDataContext();
                    List<QuestQuestion> QQ = (from o in db.QuestQuestions select o).ToList();
                    Ws.Range["D2:H13"].NumberFormat = "0.0";

                    foreach (QuestQuestion Q in QQ)
                    {
                        Ws.Cells[i + 1, 1] = i.ToString();
                        Ws.Cells[i + 1, 2] = Q.Text;


                        int AllAnswers = (from o in db.QuestAnswers where o.EmpDepNum == dep && o.QuestionId == Q.Id select o).Count();
                        Ws.Cells[i + 1, 3] = AllAnswers.ToString();
                        for (int j = 1; j < 6; j++)
                        {
                            int Answ = (from o in db.QuestAnswers where o.EmpDepNum == dep && o.QuestionId == Q.Id  && o.Result == j select o).Count();
                            Ws.Cells[i + 1, j + 3] = ((double)Answ * 100 / (double)AllAnswers);
                        }

                        i++;
                    }
                }
            }

           


        }
    }
    

