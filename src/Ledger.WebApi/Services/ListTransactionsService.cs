using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IListTransactionsService
    {
        Task<ICollection<TransactionModel>> ExecuteAsync(int skip, int take, CancellationToken cancellationToken);

        Task<ICollection<TransactionModel>> ExecuteAsync(CancellationToken cancellationToken);
    }

    public class ListTransactionsService : IListTransactionsService
    {
        private const int MaxTake = 50;

        private readonly IRequestContext _requestContext;
        private readonly IRepository _repository;

        public ListTransactionsService(
            IRequestContext requestContext,
            IRepository repository)
        {
            _requestContext = requestContext;
            _repository = repository;
        }

        public async Task<ICollection<TransactionModel>> ExecuteAsync(
            int skip,
            int take,
            CancellationToken cancellationToken)
        {
            const string sql = @"
create table #temp
(
    [Id] [uniqueidentifier] not null primary key
);

insert into #temp
select
    [Id]
from [dbo].[Transaction] 
where [UserId] = @UserId 
order by [PostedDate], [CreatedTimestamp]
offset @Skip rows
fetch next @Take rows only;

select 
    * 
from [dbo].[Transaction] t
    inner join #temp temp on t.[Id] = temp.[Id]
order by t.[PostedDate], t.[CreatedTimestamp];

select 
     p.[TransactionId] as [TransactionId]
    ,p.[Amount] as [Amount]
    ,a.[Id] as [AccountId]
    ,a.[Name] as [AccountName]
from #temp temp
    inner join [dbo].[Posting] p on temp.[Id] = p.[TransactionId]
    inner join [dbo].[Account] a on p.[AccountId] = a.[Id]
order by p.[TransactionId], p.[Id];
";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", _requestContext.UserId);
            parameters.Add("Skip", skip);
            parameters.Add("Take", take);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            return await _repository.QueryMultipleAsync(command, GetTransactionModelsAsync);
        }

        public Task<ICollection<TransactionModel>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync(0, MaxTake, cancellationToken);
        }

        private static async Task<ICollection<TransactionModel>> GetTransactionModelsAsync(SqlMapper.GridReader reader)
        {
            var transactions = (await reader.ReadAsync<Transaction>()).ToList();
            var postings = await reader.ReadAsync<PostingDto>();

            var postingsLookupByTransactionId = postings
                .GroupBy(x => x.TransactionId)
                .ToDictionary(x => x.Key, x => x.ToList());

            var transactionModels = new List<TransactionModel>(transactions.Count);

            foreach (var transaction in transactions)
            {
                var transactionModel = new TransactionModel
                {
                    Id = transaction.Id,
                    Description = transaction.Description,
                    PostedDate = transaction.PostedDate,
                };

                if (postingsLookupByTransactionId.TryGetValue(transaction.Id, out var transactionPostings))
                {
                    foreach (var posting in transactionPostings)
                    {
                        var postingModel = new PostingModel
                        {
                            Account = new AccountModel { Id = posting.AccountId, Name = posting.AccountName },
                            Amount = posting.Amount,
                        };

                        transactionModel.Postings.Add(postingModel);
                    }
                }

                transactionModels.Add(transactionModel);
            }


            return transactionModels;
        }

        private class PostingDto
        {
            public Guid TransactionId { get; set; }
            public decimal Amount { get; set; }
            public Guid AccountId { get; set; }
            public string AccountName { get; set; }
        }
    }
}