using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class ListTransactionsServiceTests : TestBase
    {
        [Test]
        public async Task ShouldGetTransactionsAsync()
        {
            var listTransactionsService = GetInstance<IListTransactionsService>();
            var transactions = await listTransactionsService.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);

            var expected = new List<TransactionModel>
            {
                new TransactionModel {Description = "Groceries", Amount = 24.87m, PostedDate = new DateTime(2020, 3, 26)},
                new TransactionModel {Description = "Netflix", Amount = 13.00m, PostedDate = new DateTime(2020, 3, 28)},
            };

            AssertTransactions(expected, transactions);
        }

        [Test]
        public async Task ShouldHandleSkipAsync()
        {
            var listTransactionsService = GetInstance<IListTransactionsService>();
            var transactions = await listTransactionsService.ExecuteAsync(1, 100, CancellationToken.None).ConfigureAwait(false);

            var expected = new List<TransactionModel>
            {
                new TransactionModel {Description = "Netflix", Amount = 13.00m, PostedDate = new DateTime(2020, 3, 28)},
            };

            AssertTransactions(expected, transactions);
        }

        [Test]
        public async Task ShouldHandleTakeAsync()
        {
            var listTransactionsService = GetInstance<IListTransactionsService>();
            var transactions = await listTransactionsService.ExecuteAsync(0, 1, CancellationToken.None).ConfigureAwait(false);

            var expected = new List<TransactionModel>
            {
                new TransactionModel {Description = "Groceries", Amount = 24.87m, PostedDate = new DateTime(2020, 3, 26)},
            };

            AssertTransactions(expected, transactions);
        }

        [Test]
        public async Task ShouldOnlyReturnTransactionsForCorrectUserAsync()
        {
            AsUserId(Guid.NewGuid());

            var listTransactionsService = GetInstance<IListTransactionsService>();
            var transactions = await listTransactionsService.ExecuteAsync(0, 1, CancellationToken.None).ConfigureAwait(false);

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