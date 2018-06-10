using System;
using Hyperion.Pf.Entity;

namespace Pixstock.Common.Model
{
    public interface IEventLog : IEntity<long>
    {
        int EventId { get; set; }

        DateTime EventDate { get; set; }

        string Sender { get; set; }

        string Message { get; set; }

        string Value { get; set; }
    }
}