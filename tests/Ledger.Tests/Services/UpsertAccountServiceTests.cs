using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class UpsertAccountServiceTests : TestBase
    {
        [Test]
        public void ShouldUpsertAccount()
        {
            var accountModel = new AccountModel {Name = "expenses:food:groceries"};

            var builder = TestBuilder.Begin()
                .AsUser()
                .UpsertAccount(accountModel)
                .ListAccounts();

            new[] {accountModel}.AssertEquals(builder.Result);
        }
    }
}