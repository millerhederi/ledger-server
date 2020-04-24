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
    public class GetPostingAggregatesRequestHandler : RequestHandler<GetPostingAggregatesRequest, GetPostingAggregatesResponse>
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public GetPostingAggregatesRequestHandler(IRepository repository, IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        protected override async Task<GetPostingAggregatesResponse> HandleAsync(GetPostingAggregatesRequest request, CancellationToken cancellationToken)
        {
            const string sql = @"
select
     sum([Amount]) as [Amount]
    ,count(1) as [Count]
    ,datepart(month, t.[PostedDate]) as [Month]
    ,datepart(year, t.[PostedDate]) as [Year]
from [dbo].[Posting] p
    inner join [dbo].[Transaction] t on p.[TransactionId] = t.[Id]
where t.[UserId] = @UserId
    and p.[AccountId] = @AccountId
group by datepart(year, t.[PostedDate]), datepart(month, t.[PostedDate])
order by [Year], [Month]";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", _requestContext.UserId);
            parameters.Add("AccountId", request.AccountId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var dtos = await _repository.QueryAsync<Dto>(command);
            var aggregates = dtos.Select(x => new GetPostingAggregatesResponse.Aggregate
                {
                    Amount = x.Amount,
                    Count = x.Count,
                    Date = new DateTime(x.Year, x.Month, 1),
                }).ToList();

            return new GetPostingAggregatesResponse
            {
                Aggregates = aggregates,
            };
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class Dto
        {
            public decimal Amount { get; set; }
            public int Count { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
        }
    }
}