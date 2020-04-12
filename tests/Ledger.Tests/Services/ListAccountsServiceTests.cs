using System.Collections.Generic;
using System.Linq;
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
                .ListAccounts();

            var expected = new List<AccountModel>
            {
                new AccountModel { FullyQualifiedName = "assets", Name = "assets", ParentFullyQualifiedName = null },
                new AccountModel { FullyQualifiedName = "assets:checking", Name = "checking", ParentFullyQualifiedName = "assets" },
                new AccountModel { FullyQualifiedName = "assets:checking:boa", Name = "boa", ParentFullyQualifiedName = "assets:checking" },
                new AccountModel { FullyQualifiedName = "assets:checking:chase", Name = "chase", ParentFullyQualifiedName = "assets:checking" },
                new AccountModel { FullyQualifiedName = "expenses", Name = "expenses", ParentFullyQualifiedName = null },
                new AccountModel { FullyQualifiedName = "expenses:food", Name = "food", ParentFullyQualifiedName = "expenses" },
                new AccountModel { FullyQualifiedName = "expenses:housing", Name = "housing", ParentFullyQualifiedName = "expenses" },
                new AccountModel { FullyQualifiedName = "expenses:transportation", Name = "transportation", ParentFullyQualifiedName = "expenses" },
                new AccountModel { FullyQualifiedName = "liabilities", Name = "liabilities", ParentFullyQualifiedName = null },
                new AccountModel { FullyQualifiedName = "liabilities:credit_card", Name = "credit_card", ParentFullyQualifiedName = "liabilities" },
                new AccountModel { FullyQualifiedName = "liabilities:credit_card:visa", Name = "visa", ParentFullyQualifiedName = "liabilities:credit_card" },
            };

            AssertAccounts(expected, builder.Result);
        }

        private static void AssertAccounts(ICollection<AccountModel> expected, ICollection<AccountModel> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Expected the counts of the two collections to equal.");

            foreach (var (expectedAccount, actualAccount) in expected.Zip(actual))
            {
                AssertAccount(expectedAccount, actualAccount);
            }
        }

        private static void AssertAccount(AccountModel expected, AccountModel actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, nameof(expected.Name));
            Assert.AreEqual(expected.FullyQualifiedName, actual.FullyQualifiedName, nameof(expected.FullyQualifiedName));
            Assert.AreEqual(expected.ParentFullyQualifiedName, actual.ParentFullyQualifiedName, nameof(expected.ParentFullyQualifiedName));
        }
    }
}