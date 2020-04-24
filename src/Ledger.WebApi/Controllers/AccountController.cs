using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IRequestProcessingPipeline _pipeline;

        public AccountController(IRequestProcessingPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        [HttpGet]
        public async Task<ResponseEnvelope<ListAccountsResponse>> ListAccountsAsync(CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(new ListAccountsRequest(), cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting")]
        public async Task<ResponseEnvelope<ListPostingsResponse>> ListAccountPostings(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(new ListPostingsRequest(accountId), cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting/monthly")]
        public async Task<ResponseEnvelope<GetPostingAggregatesResponse>> GetMonthlyAggregates(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _pipeline.ExecuteAsync(new GetPostingAggregatesRequest(accountId), cancellationToken);
        }
    }
}