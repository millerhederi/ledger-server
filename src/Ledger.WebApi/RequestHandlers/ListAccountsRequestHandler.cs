using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;

namespace Ledger.WebApi.RequestHandlers
{
    public class ListAccountsRequestHandler : RequestHandler<ListAccountsRequest, ListAccountsResponse>
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public ListAccountsRequestHandler(
            IRepository repository,
            IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        protected override async Task<ListAccountsResponse> HandleAsync(ListAccountsRequest request, CancellationToken cancellationToken)
        {
            const string sql = @"
select
    *
from [dbo].[Account] a
where a.[UserId] = @UserId
order by a.[Name]";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", _requestContext.UserId);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            var accounts = await _repository.QueryAsync<Account>(command);
            var items = accounts.Select(x => new AccountModel
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();

            return new ListAccountsResponse
            {
                Items = items,
            };
        }
    }
}