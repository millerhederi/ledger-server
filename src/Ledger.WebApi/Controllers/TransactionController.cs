using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ledger.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IGetTransactionService _getTransactionService;
        private readonly IGetTransactionsService _getTransactionsService;
        private readonly IUpsertTransactionService _upsertTransactionService;

        public TransactionController(
            IGetTransactionService getTransactionService,
            IGetTransactionsService getTransactionsService,
            IUpsertTransactionService upsertTransactionService)
        {
            _getTransactionService = getTransactionService;
            _getTransactionsService = getTransactionsService;
            _upsertTransactionService = upsertTransactionService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TransactionModel>> GetTransactionAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var transaction = await _getTransactionService.ExecuteAsync(id, cancellationToken);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        [HttpGet]
        public async Task<ICollection<TransactionModel>> GetTransactionsAsync(CancellationToken cancellationToken)
        {
            return await _getTransactionsService.ExecuteAsync(cancellationToken);
        }

        [HttpPost]
        public async Task<Guid> PostTransactionAsync(
            [FromBody] TransactionModel transaction,
            CancellationToken cancellationToken)
        {
            return await _upsertTransactionService.ExecuteAsync(transaction, cancellationToken);
        }
    }
}