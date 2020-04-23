using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<GetTransactionResponse>> GetTransactionAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetTransactionRequest(id), cancellationToken);

            if (response.Transaction == null)
            {
                return NotFound();
            }

            return response;
        }

        [HttpGet]
        public async Task<ListTransactionsResponse> ListTransactionsAsync(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ListTransactionsRequest(), cancellationToken);
        }

        [HttpPost]
        public async Task<UpsertTransactionResponse> PostTransactionAsync(
            [FromBody] UpsertTransactionRequest request,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<UpsertTransactionResponse> UpdateTransactionAsync(
            [FromRoute] Guid id,
            [FromBody] UpsertTransactionRequest request,
            CancellationToken cancellationToken)
        {
            request.Transaction.Id = id;

            return await _mediator.Send(request, cancellationToken);
        }
    }
}