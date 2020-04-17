using System.Collections.Generic;
using System.Linq;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests
{
    public static class AssertionExtensions
    {
        public static void AssertEquals(this IEnumerable<AccountModel> expected, IEnumerable<AccountModel> actual)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            Assert.AreEqual(expectedList.Count, actualList.Count, "Expected the counts of the two collections to equal.");

            foreach (var (expectedAccount, actualAccount) in expectedList.Zip(actualList))
            {
                expectedAccount.AssertEquals(actualAccount);
            }
        }

        public static void AssertEquals(this AccountModel expected, AccountModel actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, nameof(expected.Name));
        }
    }
}