using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Ledger.WebApi.Concept
{
    public interface IRequestProcessingPipeline
    {
        Task<ResponseEnvelope<TResponse>> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken);
    }

    public class RequestProcessingPipeline : IRequestProcessingPipeline
    {
        private readonly IMediator _mediator;

        public RequestProcessingPipeline(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ResponseEnvelope<TResponse>> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response == null)
            {
                return new ResponseEnvelope<TResponse>
                {
                    Error = "No response",
                };
            }

            return new ResponseEnvelope<TResponse>
            {
                Data = response,
            };
        }
    }
}