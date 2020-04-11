using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IUpsertTransactionService
    {
        Task<Guid> ExecuteAsync(TransactionModel transactionModel, CancellationToken cancellationToken);
    }

    public class UpsertTransactionService : IUpsertTransactionService
    {
        private readonly IRequestContext _requestContext;
        private readonly IRepository _repository;

        public UpsertTransactionService(IRequestContext requestContext, IRepository repository)
        {
            _requestContext = requestContext;
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(TransactionModel transactionModel, CancellationToken cancellationToken)
        {
            var transaction = new Transaction
            {
                Id = transactionModel.Id == Guid.Empty ? Guid.NewGuid() : transactionModel.Id,
                UserId = _requestContext.UserId,
                Amount = transactionModel.Amount,
                Description = transactionModel.Description,
                PostedDate = transactionModel.PostedDate,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            await _repository.UpsertAsync(new[] {transaction}, cancellationToken);

            return transaction.Id;
        }
    }
}