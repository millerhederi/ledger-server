using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Requests;
using Transaction = Ledger.WebApi.DataAccess.Transaction;

namespace Ledger.WebApi.RequestHandlers
{
    public class UpsertTransactionRequestHandler : RequestHandler<UpsertTransactionRequest, UpsertTransactionResponse>
    {
        private readonly IRequestContext _requestContext;
        private readonly IRepository _repository;

        public UpsertTransactionRequestHandler(IRequestContext requestContext, IRepository repository)
        {
            _requestContext = requestContext;
            _repository = repository;
        }

        protected override async Task<UpsertTransactionResponse> HandleAsync(UpsertTransactionRequest request, CancellationToken cancellationToken)
        {
            var currentTimestamp = DateTime.UtcNow;

            var transaction = new Transaction
            {
                Id = request.Transaction.Id == Guid.Empty ? Guid.NewGuid() : request.Transaction.Id,
                UserId = _requestContext.UserId,
                Description = request.Transaction.Description,
                PostedDate = request.Transaction.PostedDate,
                CreatedTimestamp = currentTimestamp,
                UpdatedTimestamp = currentTimestamp,
            };

            var postings = request.Transaction.Postings.Select(x => new Posting
            {
                Id = Guid.NewGuid(),
                AccountId = x.Account.Id,
                TransactionId = transaction.Id,
                Amount = x.Amount,
                CreatedTimestamp = currentTimestamp,
                UpdatedTimestamp = currentTimestamp,
            });

            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await DeletePostingsForTransactionAsync(transaction.Id, cancellationToken);

                await _repository.UpsertAsync(transaction, cancellationToken);
                await _repository.UpsertAsync(postings, cancellationToken);

                tx.Complete();
            }

            return new UpsertTransactionResponse {Id = transaction.Id};
        }

        private async Task DeletePostingsForTransactionAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            const string sql = @"delete [dbo].[Posting] where [TransactionId] = @TransactionId";

            var parameters = new DynamicParameters();
            parameters.Add("TransactionId", transactionId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            await _repository.ExecuteAsync(command);
        }
    }
}