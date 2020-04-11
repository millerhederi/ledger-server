using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class UpsertTransactionServiceTests : TestBase
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

            var upsertTransactionService = GetInstance<IUpsertTransactionService>();
            var transactionId = await upsertTransactionService.ExecuteAsync(transactionModel, CancellationToken.None);

            var getTransactionService = GetInstance<IGetTransactionService>();
            var actualTransaction = await getTransactionService.ExecuteAsync(transactionId, CancellationToken.None);

            AssertTransaction(transactionModel, actualTransaction);
        }

        [Test]
        public async Task ShouldUpdateTransaction()
        {
            var transactionModel = new TransactionModel
            {
                Amount = 3.04m,
                Description = "Tea",
                PostedDate = new DateTime(2020, 4, 2),
            };

            var upsertTransactionService = GetInstance<IUpsertTransactionService>();
            var transactionId = await upsertTransactionService.ExecuteAsync(transactionModel, CancellationToken.None);

            var updatedTransactionModel = new TransactionModel
            {
                Id = transactionId,
                Amount = 5.08m,
                Description = "Coffee",
                PostedDate = new DateTime(2020, 4, 11),
            };

            await upsertTransactionService.ExecuteAsync(updatedTransactionModel, CancellationToken.None);

            var getTransactionService = GetInstance<IGetTransactionService>();
            var actualTransaction = await getTransactionService.ExecuteAsync(transactionId, CancellationToken.None);

            AssertTransaction(updatedTransactionModel, actualTransaction);
        }

        private static void AssertTransaction(TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
        }
    }
}