using System;
using System.Linq;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class ListTransactionsServiceTests : TestBase
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

        private static readonly AccountModel DiscretionaryAccount = new AccountModel
        {
            Name = "expenses:discretionary",
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
                Description = "Netflix", 
                PostedDate = new DateTime(2020, 3, 28),
                Postings =
                {
                    new PostingModel { Amount = 14m, Account = DiscretionaryAccount },
                    new PostingModel { Amount = -14m, Account = CreditCardAccount }
                }
            },
        };

        [Test]
        public void ShouldGetTransactions()
        {
            var builder = GetBuilderWithSetupTransactions();

            var actualTransactions = builder.ListTransactions().Result;

            TransactionModels.AssertEquals(actualTransactions);
        }

        [Test]
        public void ShouldHandleSkip()
        {
            var transactions = GetBuilderWithSetupTransactions()
                .ListTransactions(1, 100)
                .Result;

            TransactionModels.Skip(1).AssertEquals(transactions);
        }

        [Test]
        public void ShouldHandleTake()
        {
            var transactions = GetBuilderWithSetupTransactions()
                .ListTransactions(0, 1)
                .Result;

            TransactionModels.Take(1).AssertEquals(transactions);
        }

        [Test]
        public void ShouldOnlyReturnTransactionsForCorrectUser()
        {
            var transactions = GetBuilderWithSetupTransactions()
                .AsUser()
                .ListTransactions()
                .Result;

            Enumerable.Empty<TransactionModel>().AssertEquals(transactions);
        }

        private static TestBuilder GetBuilderWithSetupTransactions()
        {
            var builder = TestBuilder.Begin().AsUser();

            builder
                .UpsertAccount(CreditCardAccount)
                .UpsertAccount(FoodAccount)
                .UpsertAccount(DiscretionaryAccount);

            foreach (var transaction in TransactionModels)
            {
                builder.UpsertTransaction(transaction);
            }

            return builder;
        }
    }
}