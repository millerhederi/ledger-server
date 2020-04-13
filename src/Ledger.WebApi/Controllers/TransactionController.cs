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
        private readonly IListTransactionsService _listTransactionsService;
        private readonly IUpsertTransactionService _upsertTransactionService;

        public TransactionController(
            IGetTransactionService getTransactionService,
            IListTransactionsService listTransactionsService,
            IUpsertTransactionService upsertTransactionService)
        {
            _getTransactionService = getTransactionService;
            _listTransactionsService = listTransactionsService;
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
        public async Task<ICollection<TransactionModel>> ListTransactionsAsync(CancellationToken cancellationToken)
        {
            return await _listTransactionsService.ExecuteAsync(cancellationToken);
        }

        [HttpPost]
        public async Task<Guid> PostTransactionAsync(
            [FromBody] TransactionModel transaction,
            CancellationToken cancellationToken)
        {
            return await _upsertTransactionService.ExecuteAsync(transaction, cancellationToken);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<Guid> UpdateTransactionAsync(
            [FromRoute] Guid id,
            [FromBody] TransactionModel transaction,
            CancellationToken cancellationToken)
        {
            transaction.Id = id;

            return await _upsertTransactionService.ExecuteAsync(transaction, cancellationToken);
        }
    }
}