using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests.Requests
{
    public class UpsertAccountRequestTests : TestBase
    {
        [Test]
        public void ShouldUpsertAccount()
        {
            var accountModel = new AccountModel {Name = "expenses:food:groceries"};

            var builder = TestBuilder.Begin()
                .AsUser()
                .UpsertAccount(accountModel)
                .ExecuteRequest(new ListAccountsRequest());

            new[] { accountModel }.AssertEquals(builder.Result.Items);
        }
    }
}