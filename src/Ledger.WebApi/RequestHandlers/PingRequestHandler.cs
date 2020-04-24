using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Requests;

namespace Ledger.WebApi.RequestHandlers
{
    public class PingRequestHandler : RequestHandler<PingRequest, string>
    {
        protected override Task<string> HandleAsync (PingRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult("success");
        }
    }
}