using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pixstock.Service.Model
{
    [Table("svp_EventLog")]
    public class EventLog : Infra.Model.IEventLog
    {
        public long Id { get; set; }

        public DateTime EventDate { get; set; }

        public int EventId { get; set; }

        public string Sender { get; set; }

        public string Message { get; set; }

        public string Value { get; set; }
    }
}
