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

            var fullyQualifiedNameLookupByAccountId = GetFullyQualifiedNameLookup(accounts);

            return accounts
                .Select(x => new AccountModel
                {
                    Name = x.Name,
                    ParentFullyQualifiedName = x.ParentAccountId.HasValue
                        ? fullyQualifiedNameLookupByAccountId[x.ParentAccountId.Value]
                        : null,
                    FullyQualifiedName = fullyQualifiedNameLookupByAccountId[x.Id],
                })
                .OrderBy(x => x.FullyQualifiedName)
                .ToList();
        }

        private IDictionary<Guid, string> GetFullyQualifiedNameLookup(ICollection<Account> accounts)
        {
            var result = new Dictionary<Guid, string>();

            var rootAccounts = accounts
                .Where(x => !x.ParentAccountId.HasValue)
                .ToArray();

            var accountsLookupByParentAccountId = accounts
                .Where(x => x.ParentAccountId.HasValue)
                .GroupBy(x => x.ParentAccountId)
                .ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var account in rootAccounts)
            {
                result.Add(account.Id, account.Name);

                PopulateLookupForAllChildrenAccounts(account.Id, account.Name);
            }

            return result;

            void PopulateLookupForAllChildrenAccounts(Guid parentAccountId, string parentAccountFullyQualifiedName)
            {
                if (!accountsLookupByParentAccountId.TryGetValue(parentAccountId, out var childrenAccounts))
                {
                    return;
                }

                foreach (var account in childrenAccounts)
                {
                    var accountFullyQualifiedName = $"{parentAccountFullyQualifiedName}:{account.Name}";

                    result.Add(account.Id, accountFullyQualifiedName);
                    
                    PopulateLookupForAllChildrenAccounts(account.Id, accountFullyQualifiedName);
                }
            }
        }
    }
}