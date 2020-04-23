using System.Collections.Generic;
using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests.Requests
{
    public class ListAccountsRequestTests : TestBase
    {
        [Test]
        public void ShouldListAccounts()
        {
            var builder = TestBuilder.Begin()
                .AsUser();

            var accountModels = new List<AccountModel>
            {
                new AccountModel { Name = "assets:checking:boa" },
                new AccountModel { Name = "expenses:food" },
            };

            foreach (var account in accountModels)
            {
                builder.UpsertAccount(account);
            }

            var response = builder.ExecuteRequest(new ListAccountsRequest()).Result;

            accountModels.AssertEquals(response.Items);
        }
    }
}