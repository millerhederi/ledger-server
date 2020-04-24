using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Ledger.WebApi.Concept
{
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        protected abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);

        async Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await HandleAsync(request, cancellationToken);
        }
    }
}