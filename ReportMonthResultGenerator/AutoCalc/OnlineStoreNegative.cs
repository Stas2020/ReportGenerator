using ReportMonthResultGenerator.S2010;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ReportMonthResultGenerator.OnlineStore;
using static ReportMonthResultGenerator.Zendesk;

namespace ReportMonthResultGenerator.AutoCalc
{
    // TODO:
    // Класс по сути повторяет то, что делает OnlineStoreRating, надо сделать, чтобы за раз выполнялись обе калькуляции
    class OnlineStoreNegative : CalcBase
    {
        private DateTime _beginDate;
        private DateTime _endDate;

        private List<ReportDayResult> _calculatedReports;
        private DepartmentInfo[] _departments;

        private ZendeskSearchApiClient _zendeskClient = new ZendeskSearchApiClient();

        #region для дебагового безумия
        private Dictionary<int, int> negativeCountByDep = new Dictionary<int, int>();
        private Dictionary<int, string> departmentsByName = new Dictionary<int, string>();
        private int Total { get; set; } = 0;
        private int TotalNonMoody { get; set; } = 0;
        private int TotalUnreasonable { get; set; } = 0;
        private int TotalMoodOnly { get; set; } = 0;
        private int TotalReasonOnly { get; set; } = 0;
        private int TotalNonPositive { get; set; } = 0;
        private List<ZendeskTicket> missingDepartmentTickets = new List<ZendeskTicket>();
        private Dictionary<string, int> NameToCount = new Dictionary<string, int>();
        private List<int> NotFoundDeps = new List<int>();
        #endregion
        public OnlineStoreNegative(DateTime? minDate = null, DateTime? maxDate = null)
        {
            _beginDate = minDate ?? DateTime.MinValue;
            _endDate = maxDate ?? DateTime.Now;

            _calculatedReports = new List<ReportDayResult>();

            //загружаем количество заказов
            Dictionary<DateTime, Dictionary<int, int>> orderCounts = new Dictionary<DateTime, Dictionary<int, int>>();
            for (DateTime dt = _beginDate; dt <= _endDate; dt = dt.AddDays(1))
            {
                orderCounts.Add(dt, MOZGIntegration.GetIMOrders(dt));
            }

            var client = new XrepSoapClient();
            _departments = client.GetPointList3();
            foreach (var dep in _departments)
            {
                departmentsByName[dep.Number] = dep.Name;
            }
            _zendeskClient.Departments = departmentsByName;

            //предварительно заполняем количество заказов
            foreach (var dtOrds in orderCounts) 
            {
                foreach (var depOrds in dtOrds.Value)
                {
                    var exists = _calculatedReports.FirstOrDefault(_rec => _rec.BD == dtOrds.Key && _rec.Dep == depOrds.Key);
                    if (exists == null)
                    {
                        int orderCount = depOrds.Value;
                        exists = new ReportDayResult()
                        {
                            BD = dtOrds.Key,
                            Dep = depOrds.Key,
                            Summ = 0,
                            Count = orderCount,
                            DepName = _departments.FirstOrDefault(_dep => _dep.Number == depOrds.Key)?.Name,
                            TypeId = TypeId,
                            Value = orderCount != 0 ? (double)(((double)1) / (double)orderCount) : 0 //maxRating)
                        };
                        _calculatedReports.Add(exists);
                    }
                }
            }

            var tickets = _zendeskClient.RetrieveTickets(_beginDate, _endDate, mobileAppOnly: false).Result;
            
            foreach (var ticket in tickets)
            {
                AddTicketToCalculation(ticket);
            }
            foreach (var kvp in negativeCountByDep)
            {
                if (!departmentsByName.TryGetValue(kvp.Key, out string depname))
                {
                    depname = kvp.Key.ToString();
                    NotFoundDeps.Add(kvp.Key);
                }
                NameToCount[depname] = kvp.Value;
            }
        }

        private void AddTicketToCalculation(ZendeskTicket ticket)
        {
            var date = new DateTime(ticket.created_at.Value.Year, ticket.created_at.Value.Month, ticket.created_at.Value.Day);
            var dep = 0;
            var restField = ticket.fields?.FirstOrDefault(_f => _f.id == FieldIdOf.Department);
            if (restField != null && int.TryParse(Convert.ToString(restField.value), out int iRes))
                dep = CorrectTwinDep.GetDep(iRes);
            var exists = _calculatedReports.FirstOrDefault(_rec => _rec.BD == date && _rec.Dep == dep);

            var moodField = ticket.custom_fields.FirstOrDefault(field => field.id == FieldIdOf.Mood);
            bool isNegativeMood = moodField != null && Convert.ToString(moodField.value) == Moods.Negative;

            var reasonField = ticket.custom_fields.FirstOrDefault(field => field.id == FieldIdOf.Reason);
            bool isNegativeReason = reasonField != null && Reasons.IsNegative(Convert.ToString(reasonField.value));

            bool isNegative = isNegativeMood && isNegativeReason;
            
            #region дебаговое сумасшествие
            if (moodField == null || moodField.value == null) TotalNonMoody++;
            if (reasonField == null || restField.value == null) TotalUnreasonable++;
            if (isNegative)
            {
                Total++;   
                if (negativeCountByDep.TryGetValue(dep, out var count)) 
                {
                    negativeCountByDep[dep] = count + 1;    
                }
                else
                {
                    negativeCountByDep[dep] = 1;
                }
                if (dep == 0) missingDepartmentTickets.Add(ticket);
            }
            else
            {
                if (isNegativeMood) TotalMoodOnly++;
                if (isNegativeReason) TotalReasonOnly++;
            }
            #endregion

            if (exists == null)
            {
                exists = new ReportDayResult()
                {
                    BD = date,
                    Dep = dep,
                    Summ = isNegative ? 1.0 : 0.0,
                    //Count = 1,
                    Count = 0,
                    DepName = _departments.FirstOrDefault(_dep => _dep.Number == dep)?.Name,
                    TypeId = TypeId,
                    //Value = isNegative ? 1.0 : 0.0
                    Value = 0.0
                };
                _calculatedReports.Add(exists);
            }
            else
            {
                //exists.Count += 1;
                if (isNegative)
                {
                    exists.Summ += 1;
                }
                //exists.Value = exists.Summ / (double)exists.Count;
                exists.Value = exists.Count != 0 ? exists.Summ / (double)exists.Count : 0;
            }
        }

        public override List<ReportDayResult> Calc(DateTime day)
        {
            return _calculatedReports.Where(_rec => _rec.BD == day).ToList();
        }
    }

    // Старая версия, заменено 24.10.2022
    class OnlineStoreNegativeWihMozgIntegration : CalcBase
    {
        DateTime? minDate = null;
        List<ReportDayResult> calculated = null;
        public OnlineStoreNegativeWihMozgIntegration(DateTime? _minDate)
        {
            minDate = _minDate;
        }
        public override List<ReportDayResult> Calc(DateTime day)
        {
            if (calculated == null)
            {
                calculated = new List<ReportDayResult>();

                Dictionary<DateTime, Dictionary<int, int>> orderCounts = new Dictionary<DateTime, Dictionary<int, int>>();
                for (DateTime dt = (DateTime)minDate; dt <= day; dt = dt.AddDays(1))
                {
                    orderCounts.Add(dt, MOZGIntegration.GetIMOrders(dt));
                }


                var Serv = new S2010.XrepSoapClient();
                var DepList = Serv.GetPointList3();

                //var testtttt = orderCounts.Where(_k => _k.Key >= new DateTime(2022, 06, 01) && _k.Key <= new DateTime(2022, 06, 15) && _k.Value.ContainsKey(180))
                //    .OrderBy(_k => _k.Key).Select(_k => $"{_k.Key:yyyy-MM-dd} {_k.Value[180]}");

                foreach (var dtOrds in orderCounts)
                    foreach (var depOrds in dtOrds.Value)
                    {
                        var exists = calculated.FirstOrDefault(_rec => _rec.BD == dtOrds.Key && _rec.Dep == depOrds.Key);
                        if (exists == null)
                        {
                            int orderCount = depOrds.Value;
                            exists = new ReportDayResult()
                            {
                                BD = dtOrds.Key,
                                Dep = depOrds.Key,
                                Summ = 0,
                                Count = orderCount,
                                DepName = DepList.FirstOrDefault(_dep => _dep.Number == depOrds.Key)?.Name,
                                TypeId = TypeId,
                                Value = orderCount != 0 ? (double)(((double)1) / (double)orderCount) : 0 //maxRating)
                            };
                            calculated.Add(exists);
                        }
                    }

                using (var client = new System.Net.Http.HttpClient())
                {
                    var client_id = "a.yakovleva@coffeemania.ru/token";
                    var client_secret = "hey4YvrGEpD13BMUw35CPYljBgGlt1mYUp5no2fV";

                    //var str = UTF8Encoding.Convert(Encoding.bas)

                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{client_id}:{client_secret}"))
                    );
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //client.DefaultRequestHeaders.Add("Accept", "application/json");
                    // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var data = new System.Net.Http.FormUrlEncodedContent(new[]
                    {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
                });



                    var result = "";




                    //System.Net.Http.HttpResponseMessage responseCount = client.GetAsync(@"https://coffeemania.zendesk.com/api/v2/ticket_fields").Result;
                    System.Net.Http.HttpResponseMessage responseCount = client.GetAsync(@"https://coffeemania.zendesk.com/api/v2/tickets/count").Result;
                    IMTicketCount count = null;
                    using (System.IO.StreamReader stream = new System.IO.StreamReader(responseCount.Content.ReadAsStreamAsync().Result))
                    {
                        result = stream.ReadToEnd();
                        count = Newtonsoft.Json.JsonConvert.DeserializeObject<IMTicketCount>(result);
                    }


                    //System.Net.Http.HttpResponseMessage responseTickets = client.GetAsync(@"https://coffeemania.zendesk.com/api/v2/tickets").Result;
                    long pages = count.count.value / 100;
                    pages += 3;
                    System.Net.Http.HttpResponseMessage responseTickets = client.GetAsync(@"https://coffeemania.zendesk.com/api/v2/tickets.json?page=" + pages).Result; //310

                    IMTickets tickets = null;
                    using (System.IO.StreamReader stream = new System.IO.StreamReader(responseTickets.Content.ReadAsStreamAsync().Result))
                    {
                        result = stream.ReadToEnd();
                        tickets = Newtonsoft.Json.JsonConvert.DeserializeObject<IMTickets>(result, new Newtonsoft.Json.JsonSerializerSettings() { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local });
                        responseTickets = client.GetAsync(tickets.previous_page).Result;
                    }

                    //bool hasLetterMinDate = false;
                    int pagesToRead = 3;
                    while (tickets.previous_page != null && pagesToRead > 0)//!hasLetterMinDate)
                    {
                        using (System.IO.StreamReader stream = new System.IO.StreamReader(responseTickets.Content.ReadAsStreamAsync().Result))
                        {
                            result = stream.ReadToEnd();
                            var ticketsAdd = Newtonsoft.Json.JsonConvert.DeserializeObject<IMTickets>(result, new Newtonsoft.Json.JsonSerializerSettings() { DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local });
                            tickets.tickets.AddRange(ticketsAdd.tickets);
                            tickets.previous_page = ticketsAdd.previous_page;
                            tickets.next_page = ticketsAdd.next_page;
                            tickets.count = ticketsAdd.count;

                            //if (ticketsAdd.tickets.FirstOrDefault(_t => _t.id == 75433) != null)
                            //{
                            //    var sdfsdf = ticketsAdd.tickets.FirstOrDefault(_t => _t.id == 75433);
                            //    ;
                            //}
                            //if (ticketsAdd.tickets.FirstOrDefault(_t => _t.id == 78061) != null) 
                            //{
                            //    var sdfsdf = ticketsAdd.tickets.FirstOrDefault(_t => _t.id == 78061);
                            //    ;
                            //}

                            responseTickets = client.GetAsync(tickets.previous_page).Result;

                            if (ticketsAdd.tickets.Count(_tick => _tick.created_at < minDate) > 0)
                                pagesToRead--;
                            //hasLetterMinDate = ticketsAdd.tickets.Count(_tick => _tick.created_at < minDate) > 0;
                            ////hasLetterMinDate = ticketsAdd.tickets.Count(_tick => _tick.updated_at < minDate) > 0;
                        }
                    }

                    var tickFiltered = tickets.tickets.Where(_tick => _tick.created_at >= minDate).ToList();
                    //var tickFiltered = tickets.tickets.Where(_tick => _tick.updated_at >= minDate).ToList();
                    //tickFiltered = tickets.tickets.Where(_tick => _tick.fields.Count(_f => _f.id == 360016140737 && int.TryParse(Convert.ToString(_f.value), out int i)) > 0
                    //                                           && _tick.fields.Count(_f => _f.id == 360017986557 && int.TryParse(Convert.ToString(_f.value), out int i)) > 0).ToList();

                    ///////tickFiltered = tickets.tickets.Where(_tick => _tick.fields.Count(_f => _f.id == 360017986557 && int.TryParse(Convert.ToString(_f.value), out int i)) > 0).ToList();
                    tickFiltered = tickFiltered.Where(_tick =>
                        _tick.fields.Count(_f => _f.id == 360016048758 && _f.value != null && Convert.ToString(_f.value).ToLower().IndexOf("негатив") != -1) > 0
                    //&& _tick.fields.Count(_f => _f.id == 360016557417 && _f.value != null && Convert.ToString(_f.value).ToLower() != "null") > 0
                    ).ToList();






                    /*
                         ;

                         Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                         app.Visible = true;
                         Microsoft.Office.Interop.Excel.Workbook Wb = app.Workbooks.Add(true);
                         Microsoft.Office.Interop.Excel.Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
                         Ws.Name = "Блюда";
                         int col = 3;
                         Ws.Cells[1, 1] = "Подразделение";
                         Ws.Cells[1, 2] = "Чеков";
                         Ws.Cells[1, 3] = "Тикеты все";
                         Ws.Cells[1, 4] = "Негатив";

                         //CorrectTwinDep.GetDep

                         var tickFiltered222 = tickets.tickets.Where(_tick => true).ToList();
                         Dictionary<string, List<int>> ress = new Dictionary<string, List<int>>();
                         foreach (var dp in DepList.Where(_d => _d.Enabled))
                         {
                             var ddd = tickFiltered222.Where(_t => _t.created_at >= minDate && _t.created_at < day.AddDays(1)
                             && _t.fields.FirstOrDefault(_f => _f.id == 360016140737) != null
                             && _t.fields.FirstOrDefault(_f => _f.id == 360016140737).value != null
                             && int.TryParse(_t.fields.FirstOrDefault(_f => _f.id == 360016140737).value.ToString(),out int i)
                             && CorrectTwinDep.GetDep(Convert.ToInt32(_t.fields.FirstOrDefault(_f => _f.id == 360016140737).value.ToString())).ToString() == dp.Number.ToString()).ToList();
                             var dNeg = ddd.Where(_tick =>
                             _tick.fields.Count(_f => _f.id == 360016048758 && _f.value != null && Convert.ToString(_f.value).ToLower().IndexOf("негатив") != -1) > 0).ToList();
                             if (ddd.Count > 0)
                                 ress.Add(dp.Name, new List<int>() { dp.Number, ddd.Count, dNeg.Count });
                         }
                         var resssss = ress.OrderBy(_k => _k.Key).Select(_k =>
                         {
                             return $"{_k.Key}               {_k.Value}";
                         }).ToList();

                         foreach(var rr in ress.OrderBy(_k => _k.Key))
                         {
                             Ws.Cells[col, 1] = rr.Key;
                             Ws.Cells[col, 2] = calculated.Where(_c => _c.BD >= minDate && _c.BD < day.AddDays(1) && _c.Dep == rr.Value[0]).Sum(_c => _c.Count);                        
                             Ws.Cells[col, 3] = rr.Value[1];
                             Ws.Cells[col, 4] = rr.Value[2];
                             col++;
                         }

                    */



                    foreach (var record in tickFiltered)
                    {
                        var date = new DateTime(record.created_at.Value.Year, record.created_at.Value.Month, record.created_at.Value.Day);
                        //var date = new DateTime(record.updated_at.Value.Year, record.updated_at.Value.Month, record.updated_at.Value.Day);
                        var dep = 0;
                        var restField = record.fields.FirstOrDefault(_f => _f.id == 360016140737);
                        if (restField != null && int.TryParse(Convert.ToString(restField.value), out int iRes))
                            dep = CorrectTwinDep.GetDep(iRes);



                        //var rating = Convert.ToInt32(record.fields.First(_f => _f.id == 360017986557).value);


                        var exists = calculated.FirstOrDefault(_rec => _rec.BD == date && _rec.Dep == dep);
                        if (exists == null)
                        {
                            int orderCount = 0;
                            if (orderCounts.ContainsKey(day) && orderCounts[day].ContainsKey(dep))
                                orderCount = orderCounts[day][dep];
                            exists = new ReportDayResult()
                            {
                                BD = date,
                                Dep = dep,
                                Summ = 1,
                                Count = orderCount,
                                DepName = DepList.FirstOrDefault(_dep => _dep.Number == dep)?.Name,
                                TypeId = TypeId,
                                Value = orderCount != 0 ? (double)(((double)1) / (double)orderCount) : 0 //maxRating)
                            };
                            calculated.Add(exists);
                        }
                        else
                        {
                            exists.Summ += 1;
                            exists.Value = exists.Count != 0 ? (double)(((double)exists.Summ) / (double)exists.Count) : 0;
                        }
                    }



                }

            }

            //var test = calculated.Where(_rec => _rec.BD >= new DateTime(2022, 06, 01) && _rec.BD <= new DateTime(2022, 06, 15) && _rec.Dep == 205).ToList();
            //var testS = test.Sum(_r => _r.Summ);
            //var testC = test.Sum(_r => _r.Count);

            //var testDMin = test.Max(_r => _r.BD);
            //var testDMax = test.Min(_r => _r.BD);

            return calculated.Where(_rec => _rec.BD == day).ToList(); ;
        }

    }
}
