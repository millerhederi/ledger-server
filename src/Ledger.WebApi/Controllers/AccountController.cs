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

        public AccountController(IListAccountsService listAccountsService, IListPostingsService listPostingsService)
        {
            _listAccountsService = listAccountsService;
            _listPostingsService = listPostingsService;
        }

        [HttpGet]
        public async Task<IEnumerable<AccountModel>> ListAccountsAsync(CancellationToken cancellationToken)
        {
            return await _listAccountsService.ExecuteAsync(cancellationToken);
        }

        [HttpGet]
        [Route("{accountId}/posting")]
        public async Task<ICollection<PostingModel>> ListAccountPostings(
            [FromRoute] Guid accountId,
            CancellationToken cancellationToken)
        {
            return await _listPostingsService.ExecuteAsync(accountId, cancellationToken);
        }
    }
}