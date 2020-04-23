using System;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests.Requests
{
    public class AddUserRequestTests : TestBase
    {
        [Test]
        public async Task ShouldAddUserAsync()
        {
            var userName = Guid.NewGuid().ToString();

            var builder = TestBuilder.Begin()
                .ExecuteRequest(new AddUserRequest {UserName = userName, Password = "Password123!"});

            var repository = builder.GetInstance<IRepository>();

            var user = await repository.QuerySingleOrDefaultAsync<User>(new CommandDefinition(
                "select * from [dbo].[User] where [UserName] = @UserName", 
                new {UserName = userName}));

            if (user == null)
            {
                Assert.Fail($"Expecting a user with UserName '{userName}' to exist.");
            }
        }

        [Test]
        public void ShouldThrowWhenAddingUserWithDuplicateUserName()
        {
            var request = new AddUserRequest
            {
                UserName = Guid.NewGuid().ToString(),
                Password = "Password123!",
            };

            var builder = TestBuilder.Begin().ExecuteRequest(request);

            Assert.Throws<AggregateException>(() => builder.ExecuteRequest(request));
        }
    }
}