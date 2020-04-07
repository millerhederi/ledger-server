using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class InsertTransactionServiceTests : TestBase
    {
        [Test]
        public async Task ShouldInsertTransaction()
        {
            var transactionModel = new TransactionModel
            {
                Amount = 3.04m,
                Description = "Tea",
                PostedDate = new DateTime(2020, 4, 2),
            };

            var insertTransactionService = GetInstance<IInsertTransactionService>();
            await insertTransactionService.ExecuteAsync(transactionModel, CancellationToken.None).ConfigureAwait(false);

            var getTransactionService = GetInstance<IGetTransactionsService>();
            var transactions = await getTransactionService.ExecuteAsync(2, 1, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(1, transactions.Count, "Expected exactly 1 transaction.");
            AssertTransaction(transactionModel, transactions.Single());
        }

        private static void AssertTransaction(TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
        }
    }
}