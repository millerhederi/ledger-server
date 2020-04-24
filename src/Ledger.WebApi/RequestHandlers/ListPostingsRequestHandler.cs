using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Requests;

namespace Ledger.WebApi.RequestHandlers
{
    public class ListPostingsRequestHandler : RequestHandler<ListPostingsRequest, ListPostingsResponse>
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public ListPostingsRequestHandler(IRepository repository, IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        protected override async Task<ListPostingsResponse> HandleAsync(ListPostingsRequest request, CancellationToken cancellationToken)
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
            parameters.Add("AccountId", request.AccountId);
            parameters.Add("UserId", _requestContext.UserId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var dtos = await _repository.QueryAsync<PostingDto>(command);
            var items = dtos.Select(x => new ListPostingsResponse.Posting
                {
                    Amount = x.Amount,
                    Id = x.PostingId,
                    PostedDate = x.PostedDate,
                    Description = x.Description,
                    TransactionId = x.TransactionId,
                }).ToList();

            return new ListPostingsResponse
            {
                Items = items,
            };
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