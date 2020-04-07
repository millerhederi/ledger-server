using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
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

            return await _repository.QuerySingleOrDefaultAsync<TransactionModel>(command);
        }
    }
}