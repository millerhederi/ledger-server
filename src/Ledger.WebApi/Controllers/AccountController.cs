using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ListAccountsResponse> ListAccountsAsync(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ListAccountsRequest(), cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting")]
        public async Task<ListPostingsResponse> ListAccountPostings(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ListPostingsRequest(accountId), cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting/monthly")]
        public async Task<GetPostingAggregatesResponse> GetMonthlyAggregates(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetPostingAggregatesRequest(accountId), cancellationToken);
        }
    }
}