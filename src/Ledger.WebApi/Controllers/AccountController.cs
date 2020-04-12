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

        public AccountController(IListAccountsService listAccountsService)
        {
            _listAccountsService = listAccountsService;
        }

        [HttpGet]
        public async Task<IEnumerable<AccountModel>> ListAccountsAsync(CancellationToken cancellationToken)
        {
            return await _listAccountsService.ExecuteAsync(cancellationToken);
        }
    }
}