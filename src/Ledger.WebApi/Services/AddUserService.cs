using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;

namespace Ledger.WebApi.Services
{
    public interface IAddUserService
    {
        Task<Guid> ExecuteAsync(LoginModel model, CancellationToken cancellationToken);
    }

    public class AddUserService : IAddUserService
    {
        private readonly IRepository _repository;

        public AddUserService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> ExecuteAsync(LoginModel model, CancellationToken cancellationToken)
        {
            if (await GetIsUserNameAlreadyTakenAsync(model.UserName, cancellationToken))
            {
                throw new Exception("The given UserName is already taken.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = model.UserName,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            await _repository.UpsertAsync(user, cancellationToken);

            return user.Id;
        }

        private async Task<bool> GetIsUserNameAlreadyTakenAsync(string userName, CancellationToken cancellationToken)
        {
            const string sql = @"
select
    Id
from [dbo].[User]
where [UserName] = @UserName";

            var parameters = new DynamicParameters();
            parameters.Add("UserName", userName);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var userId = await _repository.QuerySingleOrDefaultAsync<Guid?>(command);

            return userId.HasValue;
        }
    }
}