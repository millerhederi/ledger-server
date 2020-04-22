using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests
{
    public static class AssertionExtensions
    {
        #region AccountModel
        public static void AssertEquals(this IEnumerable<AccountModel> expected, IEnumerable<AccountModel> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this AccountModel expected, AccountModel actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, nameof(expected.Name));
        }
        #endregion AccountModel

        #region TransactionModel
        public static void AssertEquals(this IEnumerable<TransactionModel> expected, IEnumerable<TransactionModel> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);

            SortedPostings(expected.Postings).AssertEquals(SortedPostings(actual.Postings));

            static IEnumerable<PostingModel> SortedPostings(IEnumerable<PostingModel> postings)
            {
                // Try to make sorting deterministic so that we can assert on the collections
                return postings.OrderBy(x => x.Amount);
            }
        }
        #endregion TransactionModel

        #region PostingModel
        public static void AssertEquals(this IEnumerable<PostingModel> expected, IEnumerable<PostingModel> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this PostingModel expected, PostingModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            expected.Account.AssertEquals(actual.Account);
        }
        #endregion PostingModel

        #region AccountPostingModel

        public static void AssertEquals(this IEnumerable<AccountPostingModel> expected,  IEnumerable<AccountPostingModel> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this AccountPostingModel expected, AccountPostingModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
            Assert.AreEqual(expected.TransactionId, actual.TransactionId);
        }

        #endregion

        private static void AssertCollectionsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, Action<T, T> assertionFn)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            Assert.AreEqual(expectedList.Count, actualList.Count, "Expecting the count of the two collections to equal.");

            foreach (var (e, a) in expectedList.Zip(actualList))
            {
                assertionFn(e, a);
            }
        }
    }
}