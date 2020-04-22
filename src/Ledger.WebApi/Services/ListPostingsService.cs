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
        Task<ICollection<AccountPostingModel>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken);
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

        public async Task<ICollection<AccountPostingModel>> ExecuteAsync(Guid accountId, CancellationToken cancellationToken)
        {
            const string sql = @"
select
	 t.[PostedDate]
	,t.[Description]
	,p.[Amount]
	,p.[Id] as [PostingId]
    ,t.[Id] as [TransactionId]
from [dbo].[Posting] p
	inner join [dbo].[Account] a on p.[AccountId] = a.[Id]
	inner join [dbo].[Transaction] t on p.[TransactionId] = t.[Id] 
where t.[UserId] = @UserId
    and a.Id = @AccountId
order by t.[PostedDate] desc, t.[Id], p.[Id]";

            var parameters = new DynamicParameters();
            parameters.Add("AccountId", accountId);
            parameters.Add("UserId", _requestContext.UserId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var postings = await _repository.QueryAsync<PostingDto>(command);

            return postings.Select(x => new AccountPostingModel
            {
                Amount = x.Amount,
                Id = x.PostingId,
                PostedDate = x.PostedDate,
                Description = x.Description,
                TransactionId = x.TransactionId,
            }).ToList();
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class PostingDto
        {
            public decimal Amount { get; set; }
            public DateTime PostedDate { get; set; }
            public string Description { get; set; }
            public Guid PostingId { get; set; }
            public Guid TransactionId { get; set; }
        }
    }
}