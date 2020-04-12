using System;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class UpsertTransactionServiceTests : TestBase
    {
        [Test]
        public void ShouldInsertTransaction()
        {
            var transactionModel = new TransactionModel
            {
                Description = "Tea",
                PostedDate = new DateTime(2020, 4, 2),
            };

            var builder = TestBuilder.Begin().AsUser();
            var transactionId = builder
                .UpsertTransaction(transactionModel)
                .Result;
            var actualTransaction = builder
                .GetTransaction(transactionId)
                .Result;

            AssertTransaction(transactionModel, actualTransaction);
        }

        [Test]
        public void ShouldUpdateTransaction()
        {
            var transactionModel = new TransactionModel
            {
                Description = "Tea",
                PostedDate = new DateTime(2020, 4, 2),
            };

            var builder = TestBuilder.Begin().AsUser();
            var transactionId = builder
                .UpsertTransaction(transactionModel)
                .Result;

            var updatedTransactionModel = new TransactionModel
            {
                Id = transactionId,
                Description = "Coffee",
                PostedDate = new DateTime(2020, 4, 11),
            };

            var actualTransaction = builder
                .UpsertTransaction(updatedTransactionModel)
                .GetTransaction(transactionId)
                .Result;

            AssertTransaction(updatedTransactionModel, actualTransaction);
        }

        private static void AssertTransaction(TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
        }
    }
}