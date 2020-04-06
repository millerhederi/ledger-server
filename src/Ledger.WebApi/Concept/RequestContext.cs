using System;

namespace Ledger.WebApi.Concept
{
    public interface IRequestContext
    {
        bool IsAuthenticated { get; set; }

        Guid UserId { get; set; }
    }

    public class RequestContext : IRequestContext
    {
        public bool IsAuthenticated { get; set; }

        public Guid UserId { get; set; }
    }
}