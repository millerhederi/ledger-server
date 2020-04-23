using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests
{
    public static class AssertionExtensions
    {
        public static void AssertEquals(this IEnumerable<AccountModel> expected, IEnumerable<AccountModel> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this AccountModel expected, AccountModel actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, nameof(expected.Name));
        }

        public static void AssertEquals(this IEnumerable<TransactionModel> expected, IEnumerable<TransactionModel> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);

            SortedPostings(expected.Postings).AssertEquals(SortedPostings(actual.Postings));

            static IEnumerable<TransactionModel.Posting> SortedPostings(IEnumerable<TransactionModel.Posting> postings)
            {
                // Try to make sorting deterministic so that we can assert on the collections
                return postings.OrderBy(x => x.Amount);
            }
        }

        public static void AssertEquals(this IEnumerable<TransactionModel.Posting> expected, IEnumerable<TransactionModel.Posting> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this TransactionModel.Posting expected, TransactionModel.Posting actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            
            expected.Account.AssertEquals(actual.Account);
        }

        public static void AssertEquals(this TransactionModel.Account expected, TransactionModel.Account actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
        }

        public static void AssertEquals(this IEnumerable<ListPostingsResponse.Posting> expected,  IEnumerable<ListPostingsResponse.Posting> actual)
        {
            AssertCollectionsEqual(expected, actual, (e, a) => e.AssertEquals(a));
        }

        public static void AssertEquals(this ListPostingsResponse.Posting expected, ListPostingsResponse.Posting actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
            Assert.AreEqual(expected.TransactionId, actual.TransactionId);
        }

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