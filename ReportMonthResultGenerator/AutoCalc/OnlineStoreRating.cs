using Newtonsoft.Json;
using ReportMonthResultGenerator.S2010;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using static ReportMonthResultGenerator.Zendesk;

namespace ReportMonthResultGenerator.AutoCalc
{
    class OnlineStoreRating : CalcBase
    {
        private DateTime _beginDate;
        private DateTime _endDate;

        private List<ReportDayResult> _calculatedReports;
        private DepartmentInfo[] _departments;

        private ZendeskSearchApiClient _znedeskClient = new ZendeskSearchApiClient();

        public OnlineStoreRating(DateTime? minDate = null, DateTime? maxDate = null)
        {
            _beginDate = minDate ?? DateTime.MinValue;
            _endDate = maxDate ?? DateTime.Now;

            _calculatedReports = new List<ReportDayResult>();

            var client = new XrepSoapClient();
            _departments = client.GetPointList3();

            var tickets = _znedeskClient.RetrieveTickets(_beginDate, _endDate).Result;
            foreach (var ticket in tickets)
            {
                AddTicketToCalculation(ticket);
            }
        }

        private void AddTicketToCalculation(ZendeskTicket ticket)
        {
            var date = new DateTime(ticket.created_at.Value.Year, ticket.created_at.Value.Month, ticket.created_at.Value.Day);
            var dep = 0;
            var restField = ticket.fields.FirstOrDefault(_f => _f.id == FieldIdOf.Department);
            if (restField != null && int.TryParse(Convert.ToString(restField.value), out int iRes))
                dep = CorrectTwinDep.GetDep(iRes);

            //2022-11-29
            //var rating = Convert.ToInt32(ticket.fields.First(_f => _f.id == FieldIdOf.Rating).value);
            int.TryParse(Convert.ToString(ticket.fields.First(_f => _f.id == FieldIdOf.Rating).value), out int rating);
            if (rating == 0)
            {
                Console.WriteLine(Convert.ToString(ticket.fields.First(_f => _f.id == FieldIdOf.Rating).value));
            }

            var exists = _calculatedReports.FirstOrDefault(_rec => _rec.BD == date && _rec.Dep == dep);
            if (exists == null)
            {
                exists = new ReportDayResult()
                {
                    BD = date,
                    Dep = dep,
                    Summ = rating,
                    Count = 1, //maxRating,
                    DepName = _departments.FirstOrDefault(_dep => _dep.Number == dep)?.Name,
                    TypeId = TypeId,
                    Value = rating / 1.0  //maxRating
                };
                _calculatedReports.Add(exists);
            }
            else
            {
                exists.Count += 1; // maxRating;
                exists.Summ += rating;
                exists.Value = exists.Summ / (double)exists.Count;
            }
        }

        public override List<ReportDayResult> Calc(DateTime day)
        {
            return _calculatedReports.Where(_rec => _rec.BD == day).ToList();
        }
    }
}
