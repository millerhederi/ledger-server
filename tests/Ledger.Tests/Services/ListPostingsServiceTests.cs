using System;
using System.Linq;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class ListPostingsServiceTests : TestBase
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

        private static readonly TransactionModel[] TransactionModels =
        {
            new TransactionModel
            {
                Description = "Groceries",
                PostedDate = new DateTime(2020, 3, 26),
                Postings =
                {
                    new PostingModel { Amount = 87.34m, Account = FoodAccount },
                    new PostingModel { Amount = -87.34m, Account = CreditCardAccount }
                }
            },
            new TransactionModel
            {
                Description = "Pizza",
                PostedDate = new DateTime(2020, 3, 28),
                Postings =
                {
                    new PostingModel { Amount = 14m, Account = FoodAccount },
                    new PostingModel { Amount = -14m, Account = CreditCardAccount }
                }
            },
        };

        [Test]
        public void ShouldListPostings()
        {
            var postings = GetBuilderWithSetupTransactions()
                .ListPostings(FoodAccount.Id)
                .Result;

            var expectedPostings = TransactionModels
                .SelectMany(x => x.Postings)
                .Where(x => x.Account.Id == FoodAccount.Id);

            expectedPostings.AssertEquals(postings);
        }

        private static TestBuilder GetBuilderWithSetupTransactions()
        {
            var builder = TestBuilder.Begin().AsUser();

            builder
                .UpsertAccount(CreditCardAccount)
                .UpsertAccount(FoodAccount);

            foreach (var transaction in TransactionModels)
            {
                builder.UpsertTransaction(transaction);
            }

            return builder;
        }
    }
}