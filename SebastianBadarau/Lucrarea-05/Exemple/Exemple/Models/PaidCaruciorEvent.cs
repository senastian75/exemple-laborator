using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.Models.Carucior;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class PaidCaruciorEvent
    {
        public interface IPaidCarucioredEvent { }

        public record PaidCaruciorScucceededEvent : IPaidCarucioredEvent
        {
            public DateTime PublishedDate { get; }
            public string Csv { get; }
            internal PaidCaruciorScucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record PaidCaruciorFaildEvent : IPaidCarucioredEvent
        {
            public string Reason { get; }

            internal PaidCaruciorFaildEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
