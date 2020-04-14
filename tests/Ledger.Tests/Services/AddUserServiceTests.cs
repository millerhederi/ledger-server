using System;
using System.Threading.Tasks;
using Dapper;
using Ledger.WebApi.Concept;
using Ledger.WebApi.DataAccess;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class AddUserServiceTests : TestBase
    {
        [Test]
        public async Task ShouldAddUserAsync()
        {
            var userName = Guid.NewGuid().ToString();

            var builder = TestBuilder.Begin()
                .AddUser(new LoginModel {UserName = userName, Password = "Password123!"});

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
            var model = new LoginModel
            {
                UserName = Guid.NewGuid().ToString(),
                Password = "Password123!",
            };

            var builder = TestBuilder.Begin().AddUser(model);

            Assert.Throws<AggregateException>(() => builder.AddUser(model));
        }
    }
}