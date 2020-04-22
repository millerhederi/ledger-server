using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IListAccountsService _listAccountsService;
        private readonly IListPostingsService _listPostingsService;
        private readonly IGetPostingTotalsByMonthService _getPostingTotalsByMonthService;

        public AccountController(
            IListAccountsService listAccountsService, 
            IListPostingsService listPostingsService,
            IGetPostingTotalsByMonthService getPostingTotalsByMonthService)
        {
            _listAccountsService = listAccountsService;
            _listPostingsService = listPostingsService;
            _getPostingTotalsByMonthService = getPostingTotalsByMonthService;
        }

        [HttpGet]
        public async Task<IEnumerable<AccountModel>> ListAccountsAsync(CancellationToken cancellationToken)
        {
            return await _listAccountsService.ExecuteAsync(cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting")]
        public async Task<ICollection<AccountPostingModel>> ListAccountPostings(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _listPostingsService.ExecuteAsync(accountId, cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting/monthly")]
        public async Task<ICollection<MonthlyPostingTotalModel>> GetMonthlyAggregates(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _getPostingTotalsByMonthService.ExecuteAsync(accountId, cancellationToken);
        }
    }
}