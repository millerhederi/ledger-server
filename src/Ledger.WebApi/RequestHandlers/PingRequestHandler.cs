using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Requests;
using MediatR;

namespace Ledger.WebApi.RequestHandlers
{
    public class PingRequestHandler : IRequestHandler<PingRequest, string>
    {
        public Task<string> Handle(PingRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult("pong");
        }
    }
}