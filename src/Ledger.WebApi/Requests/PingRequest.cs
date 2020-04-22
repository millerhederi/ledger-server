using MediatR;

namespace Ledger.WebApi.Requests
{
    public class PingRequest : IRequest<string>
    {
    }
}