using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept.Logging;
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
        private readonly IRequestContext _requestContext;
        private readonly ITelemetryEventLogger _telemetryEventLogger;

        public RequestProcessingPipeline(
            IMediator mediator, 
            IRequestContext requestContext,
            ITelemetryEventLogger telemetryEventLogger)
        {
            _mediator = mediator;
            _requestContext = requestContext;
            _telemetryEventLogger = telemetryEventLogger;
        }

        public async Task<ResponseEnvelope<TResponse>> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken)
        {
            _requestContext.TelemetryEvent["Request"] = new
            {
                Type = request.GetType().Name,
                Data = request,
            };

            var response = await ExecuteRequestHandlerAsync(request, cancellationToken);

            _requestContext.TelemetryEvent["Response"] = response;

            await _telemetryEventLogger.LogAsync(_requestContext.TelemetryEvent);

            return response;
        }

        private async Task<ResponseEnvelope<TResponse>> ExecuteRequestHandlerAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(request, cancellationToken);

                if (response == null)
                {
                    return ResponseEnvelope<TResponse>.ErrorResponse("Empty response");
                }

                return ResponseEnvelope<TResponse>.OkResponse(response);
            }
            catch (TaskCanceledException)
            {
                // We specifically want to ignore this type of exception in the telemetry

                return ResponseEnvelope<TResponse>.ErrorResponse("Request cancelled");
            }
            catch (Exception e)
            {
                _requestContext.TelemetryEvent.AddException(e);

                return ResponseEnvelope<TResponse>.ErrorResponse("Unhandled exception");
            }
        }
    }
}