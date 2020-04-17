using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IUpsertAccountService
    {
        Task<Guid> ExecuteAsync(AccountModel model, CancellationToken cancellationToken);
    }

    public class UpsertAccountService : IUpsertAccountService
    {
        private readonly IRepository _repository;
        private readonly IRequestContext _requestContext;

        public UpsertAccountService(IRepository repository, IRequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
        }

        public async Task<Guid> ExecuteAsync(AccountModel model, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
                Name = model.Name,
                UserId = _requestContext.UserId,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            await _repository.UpsertAsync(account, cancellationToken);

            return account.Id;
        }
    }
}