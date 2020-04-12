using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class ListTransactionsServiceTests : TestBase
    {
        [Test]
        public void ShouldGetTransactions()
        {
            var transactions = TestBuilder.Begin()
                .ListTransactions()
                .Result;

            var expected = new List<TransactionModel>
            {
                new TransactionModel {Description = "Groceries", Amount = 24.87m, PostedDate = new DateTime(2020, 3, 26)},
                new TransactionModel {Description = "Netflix", Amount = 13.00m, PostedDate = new DateTime(2020, 3, 28)},
            };

            AssertTransactions(expected, transactions);
        }

        [Test]
        public void ShouldHandleSkip()
        {
            var transactions = TestBuilder.Begin()
                .ListTransactions(1, 100)
                .Result;

            var expected = new List<TransactionModel>
            {
                new TransactionModel {Description = "Netflix", Amount = 13.00m, PostedDate = new DateTime(2020, 3, 28)},
            };

            AssertTransactions(expected, transactions);
        }

        [Test]
        public void ShouldHandleTake()
        {
            var transactions = TestBuilder.Begin()
                .ListTransactions(0, 1)
                .Result;

            var expected = new List<TransactionModel>
            {
                new TransactionModel {Description = "Groceries", Amount = 24.87m, PostedDate = new DateTime(2020, 3, 26)},
            };

            AssertTransactions(expected, transactions);
        }

        [Test]
        public void ShouldOnlyReturnTransactionsForCorrectUser()
        {
            var transactions = TestBuilder.Begin()
                .AsUser(Guid.NewGuid())
                .ListTransactions()
                .Result;

            AssertTransactions(new List<TransactionModel>(), transactions);
        }

        private static void AssertTransactions(ICollection<TransactionModel> expected, ICollection<TransactionModel> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Expecting the count of the two collections to equal.");

            for (var i = 0; i < expected.Count; i++)
            {
                AssertTransaction(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        private static void AssertTransaction(TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
        }
    }
}