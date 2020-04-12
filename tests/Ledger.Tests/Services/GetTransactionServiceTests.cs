using System;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class GetTransactionServiceTests : TestBase
    {
        private static readonly AccountModel CreditCardAccount = new AccountModel
        {
            Name = "liabilities:cc:visa",
            Id = Guid.NewGuid(),
        };

        private static readonly AccountModel FoodAccount = new AccountModel
        {
            Name = "expenses:food",
            Id = Guid.NewGuid(),
        };

        private static readonly TransactionModel TransactionModel = new TransactionModel
        {
            Description = "Tea",
            PostedDate = new DateTime(2020, 4, 2),
            Postings =
            {
                new PostingModel { Amount = 3.73m, Account = FoodAccount },
                new PostingModel { Amount = -3.73m, Account = CreditCardAccount },
            }
        };

        [Test]
        public void ShouldGetTransaction()
        {
            var builder = GetBuilderWithSetupTransaction();
            var transactionId = builder.Result;
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
            var builder = GetBuilderWithSetupTransaction();
            var transactionId = builder.Result;

            var transaction = builder
                .AsUser()
                .GetTransaction(transactionId)
                .Result;

            Assert.IsNull(transaction);
        }

        private static TestBuilder<Guid> GetBuilderWithSetupTransaction()
        {
            return TestBuilder.Begin()
                .AsUser()
                .UpsertAccount(CreditCardAccount)
                .UpsertAccount(FoodAccount)
                .UpsertTransaction(TransactionModel);
        }
    }
}