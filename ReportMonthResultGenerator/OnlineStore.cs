using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;


namespace ReportMonthResultGenerator
{
    // Используется только в старой версии OnlineStoreNegative
    static class OnlineStore
    {

        [Serializable]
        public class IMTicketCount
        {
            [Serializable]
            public class IMTicketCounеType
            {
                public long value { get; set; }
                public DateTime? refreshed_at { get; set; }
            }
            public IMTicketCounеType count { get; set; }
        }
        [Serializable]
        public class IMTicketFields
        {
            [Serializable]
            public class IMTicketField
            {
                public long id { get; set; }
                public string title { get; set; }
                public long position { get; set; }
            }
            public List<IMTicketField> ticket_fields { get; set; }
        }

        [Serializable]
        public class IMTickets
        {
            [Serializable]
            public class Satisfaction_rating
            {
                public object score;
            }
            [Serializable]
            public class IdValue
            {
                public long id;
                public object value;
            }
            [Serializable]
            public class Via
            {
                public string channel;
                public object source;
            }
            public class IMTicket
            {
                public string url;
                public long? id;
                public long? external_id;
                public Via via;
                public DateTime? created_at;
                public DateTime? updated_at;
                public string type;
                public string subject;
                public string raw_subject;
                public string description;
                public string priority;
                public string status;
                public object recipient;
                public long? requester_id;
                public long? submitter_id;
                public long? assignee_id;
                public long? organization_id;
                public long? group_id;
                public object collaborator_ids;
                public object follower_ids;
                public object email_cc_ids;
                public object forum_topic_id;
                public object problem_id;
                public bool? has_incidents;
                public bool? is_public;
                public object due_at;
                public List<string> tags;
                public List<IdValue> custom_fields;

                public Satisfaction_rating satisfaction_rating;
                //public object satisfaction_rating;

                public List<object> sharing_agreement_ids;

                public List<IdValue> fields;
                //public List<object> fields;

                public List<object> followup_ids;
                public long? ticket_form_id;
                public long? brand_id;
                public bool? allow_channelback;
                public bool? allow_attachments;
            }
            public List<IMTicket> tickets { get; set; }
            public string next_page { get; set; }
            public string previous_page { get; set; }
            public int count { get; set; }
        }

    }

}
