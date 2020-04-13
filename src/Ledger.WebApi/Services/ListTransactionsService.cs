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
select 
    * 
from [dbo].[Transaction] 
where [UserId] = @UserId 
order by PostedDate, CreatedTimestamp
offset @Skip rows
fetch next @Take rows only";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", _requestContext.UserId);
            parameters.Add("Skip", skip);
            parameters.Add("Take", take);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var transactions = await _repository.QueryAsync<Transaction>(command);

            return transactions
                .Select(x => new TransactionModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Description = x.Description,
                    PostedDate = x.PostedDate,
                })
                .ToList();
        }

        public Task<ICollection<TransactionModel>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync(0, MaxTake, cancellationToken);
        }
    }
}