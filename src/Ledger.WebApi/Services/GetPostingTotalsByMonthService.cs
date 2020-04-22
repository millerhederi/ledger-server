using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IGetPostingTotalsByMonthService
    {
        Task<ICollection<MonthlyPostingTotalModel>> ExecuteAsync(Guid accountId,  CancellationToken cancellationToken);
    }

    public class GetPostingTotalsByMonthService : IGetPostingTotalsByMonthService
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public GetPostingTotalsByMonthService(IRepository repository, IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        public async Task<ICollection<MonthlyPostingTotalModel>> ExecuteAsync(
            Guid accountId,
            CancellationToken cancellationToken)
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
            parameters.Add("AccountId", accountId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var dtos = await _repository.QueryAsync<Dto>(command);

            return dtos.Select(x => new MonthlyPostingTotalModel
            {
                Amount = x.Amount,
                Count = x.Count,
                Date = new DateTime(x.Year, x.Month, 1),
            }).ToList();
        }

        private class Dto
        {
            public decimal Amount { get; set; }
            public int Count { get; set; }
            public int Month { get; set; }
            public int Year { get; set; }
        }
    }
}