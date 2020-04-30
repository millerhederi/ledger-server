using System;
using Ledger.WebApi.Concept.Logging;

namespace Ledger.WebApi.Concept
{
    public interface IRequestContext
    {
        bool IsAuthenticated { get; set; }

        Guid UserId { get; set; }

        TelemetryEvent TelemetryEvent { get; set; }
    }

    public class RequestContext : IRequestContext
    {
        public bool IsAuthenticated { get; set; }

        public Guid UserId { get; set; }

        public TelemetryEvent TelemetryEvent { get; set; }
    }
}