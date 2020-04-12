using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IGetTransactionService
    {
        Task<TransactionModel> ExecuteAsync(Guid id, CancellationToken cancellationToken);
    }

    public class GetTransactionService : IGetTransactionService
    {
        private readonly IRequestContext _requestContext;
        private readonly IRepository _repository;

        public GetTransactionService(
            IRequestContext requestContext,
            IRepository repository)
        {
            _requestContext = requestContext;
            _repository = repository;
        }

        public async Task<TransactionModel> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var transaction = await GetTransactionAsync(id, cancellationToken);

            if (transaction == null)
            {
                return null;
            }

            var transactionModel = new TransactionModel
            {
                Id = transaction.Id,
                PostedDate = transaction.PostedDate,
                Description = transaction.Description,
            };

            var postings = await GetPostingsAsync(id, cancellationToken);

            foreach (var posting in postings)
            {
                transactionModel.Postings.Add(new PostingModel 
                {
                    Amount = posting.Amount,
                    Account = new AccountModel
                    {
                        Name = posting.AccountName,
                        Id = posting.AccountId,
                    },
                });
            }

            return transactionModel;
        }

        private async Task<Transaction> GetTransactionAsync(Guid id, CancellationToken cancellationToken)
        {
            const string sql = @"
select
    *
from [dbo].[Transaction]
where UserId = @UserId
    and Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", _requestContext.UserId);
            parameters.Add("Id", id);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            return await _repository.QuerySingleOrDefaultAsync<Transaction>(command);
        }

        private async Task<IEnumerable<PostingDto>> GetPostingsAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            const string sql = @"
select
     p.[Amount]
    ,a.[Name] as [AccountName]
    ,a.[Id] as [AccountId]
from [dbo].[Posting] p
    inner Join [dbo].[Account] a on p.[AccountId] = a.[Id]
where p.[TransactionId] = @TransactionId";

            var parameters = new DynamicParameters();
            parameters.Add("TransactionId", transactionId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            return await _repository.QueryAsync<PostingDto>(command);
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class PostingDto
        {
            public string AccountName { get; set; }
            public Guid AccountId { get; set; }
            public decimal Amount { get; set; }
        }
    }
}