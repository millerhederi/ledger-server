using Ledger.WebApi.Concept;

namespace Ledger.WebApi.Requests
{
    public class PingRequest : IRequest<string>
    {
        public bool Validate()
        {
            return true;
        }
    }
}