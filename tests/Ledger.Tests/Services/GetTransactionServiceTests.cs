using System;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class GetTransactionServiceTests : TestBase
    {
        private static readonly TransactionModel TransactionModel = new TransactionModel
        {
            Amount = 3.04m,
            Description = "Tea",
            PostedDate = new DateTime(2020, 4, 2),
        };

        [Test]
        public void ShouldGetTransaction()
        {
            var builder = TestBuilder.Begin().AsUser();
            var transactionId = builder.UpsertTransaction(TransactionModel).Result;
            var actualTransaction = builder.GetTransaction(transactionId).Result;

            TransactionModel.AssertEquals(actualTransaction);
        }

        [Test]
        public void ShouldHandleInvalidId()
        {
            var builder = TestBuilder.Begin()
                .AsUser()
                .GetTransaction(Guid.NewGuid());

            Assert.IsNull(builder.Result);
        }

        [Test]
        public void ShouldHandleGettingValidTransactionWithInvalidUser()
        {
            var builder = TestBuilder.Begin();

            var transactionId = builder
                .AsUser()
                .UpsertTransaction(TransactionModel)
                .Result;

            var transaction = builder
                .AsUser()
                .GetTransaction(transactionId)
                .Result;

            Assert.IsNull(transaction);
        }
    }
}