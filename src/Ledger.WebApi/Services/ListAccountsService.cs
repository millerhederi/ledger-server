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
    public interface IListAccountsService
    {
        Task<ICollection<AccountModel>> ExecuteAsync(CancellationToken cancellationToken);
    }

    public class ListAccountsService : IListAccountsService
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public ListAccountsService(
            IRepository repository,
            IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        public async Task<ICollection<AccountModel>> ExecuteAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
select
    *
from [dbo].[Account] a
where a.[UserId] = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", _requestContext.UserId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            var accounts = (await _repository.QueryAsync<Account>(command)).ToList();

            if (accounts.Count == 0)
            {
                return new List<AccountModel>();
            }

            return accounts
                .Select(x => new AccountModel
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}