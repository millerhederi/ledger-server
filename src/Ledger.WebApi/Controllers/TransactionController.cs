using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IRequestProcessingPipeline _pipeline;

        public TransactionController(IRequestProcessingPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResponseEnvelope<GetTransactionResponse>> GetTransactionAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(new GetTransactionRequest(id), cancellationToken);
        }

        [HttpGet]
        public async Task<ResponseEnvelope<ListTransactionsResponse>> ListTransactionsAsync(CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(new ListTransactionsRequest(), cancellationToken);
        }

        [HttpPost]
        public async Task<ResponseEnvelope<UpsertTransactionResponse>> PostTransactionAsync(
            [FromBody] UpsertTransactionRequest request,
            CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(request, cancellationToken);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ResponseEnvelope<UpsertTransactionResponse>> UpdateTransactionAsync(
            [FromRoute] Guid id,
            [FromBody] UpsertTransactionRequest request,
            CancellationToken cancellationToken)
        {
            request.Transaction.Id = id;

            return await _pipeline.ExecuteAsync(request, cancellationToken);
        }
    }
}