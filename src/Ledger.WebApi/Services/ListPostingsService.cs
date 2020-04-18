using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IListPostingsService
    {
        Task<ICollection<PostingModel>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken);
    }

    public class ListPostingsService : IListPostingsService
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public ListPostingsService(IRepository repository, IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        public async Task<ICollection<PostingModel>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken)
        {
            const string sql = @"
select
	 t.[PostedDate]
	,t.[Description]
	,p.[Amount]
	,a.[Name] as [AccountName]
    ,a.[Id] as [AccountId]
from [dbo].[Posting] p
	inner join [dbo].[Account] a on p.[AccountId] = a.[Id]
	inner join [dbo].[Transaction] t on p.[TransactionId] = t.[Id] 
where t.[UserId] = @UserId
    and a.Id = @AccountId";

            var parameters = new DynamicParameters();
            parameters.Add("AccountId", accountId);
            parameters.Add("UserId", _requestContext.UserId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var postings = await _repository.QueryAsync<PostingDto>(command);

            return postings.Select(x => new PostingModel
            {
                Amount = x.Amount,
                Account = new AccountModel {Name = x.AccountName, Id = x.AccountId}
            }).ToList();
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class PostingDto
        {
            public Guid AccountId { get; set; }
            public string AccountName { get; set; }
            public decimal Amount { get; set; }
        }
    }
}