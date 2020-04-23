using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Requests;
using MediatR;

namespace Ledger.WebApi.RequestHandlers
{
    public class AddUserRequestHandler : IRequestHandler<AddUserRequest, AddUserResponse>
    {
        private readonly IRepository _repository;

        public AddUserRequestHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddUserResponse> Handle(AddUserRequest request, CancellationToken cancellationToken)
        {
            if (await GetIsUserNameAlreadyTakenAsync(request.UserName, cancellationToken))
            {
                throw new Exception("The given UserName is already taken.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            await _repository.UpsertAsync(user, cancellationToken);

            return new AddUserResponse { Id = user.Id };
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