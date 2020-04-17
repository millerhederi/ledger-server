using System.Collections.Generic;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class ListAccountsServiceTests : TestBase
    {
        [Test]
        public void ShouldGetAccounts()
        {
            var builder = TestBuilder.Begin()
                .AsUser();

            var accountModels = new List<AccountModel>
            {
                new AccountModel { Name = "assets:checking:boa" },
                new AccountModel { Name = "expenses:food" },
                new AccountModel { Name = "expenses:housing" },
                new AccountModel { Name = "expenses:transportation" },
                new AccountModel { Name = "liabilities:cc:visa" },
            };

            foreach (var account in accountModels)
            {
                builder.UpsertAccount(account);
            }

            var actualAccounts = builder.ListAccounts().Result;

            accountModels.AssertEquals(actualAccounts);
        }
    }
}