using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Requests;

namespace Ledger.WebApi.RequestHandlers
{
    public class UpsertAccountRequestHandler : RequestHandler<UpsertAccountRequest, UpsertAccountResponse>
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public UpsertAccountRequestHandler(IRepository repository, IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        protected override async Task<UpsertAccountResponse> HandleAsync(UpsertAccountRequest request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = request.Account.Id == Guid.Empty ? Guid.NewGuid() : request.Account.Id,
                Name = request.Account.Name,
                UserId = _requestContext.UserId,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            await _repository.UpsertAsync(account, cancellationToken);

            return new UpsertAccountResponse {Id = account.Id};
        }
    }
}