using System;
using System.Linq;
using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests.Requests
{
    public class ListTransactionsRequestTests : TestBase
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
                    new TransactionModel.Posting { Amount = 87.34m, Account = new TransactionModel.Account {Id = FoodAccount.Id, Name = FoodAccount.Name}},
                    new TransactionModel.Posting { Amount = -87.34m, Account = new TransactionModel.Account {Id = CreditCardAccount.Id, Name = CreditCardAccount.Name}},
                }
            },
            new TransactionModel
            {
                Description = "Netflix", 
                PostedDate = new DateTime(2020, 3, 28),
                Postings =
                {
                    new TransactionModel.Posting { Amount = 14m, Account = new TransactionModel.Account {Id = DiscretionaryAccount.Id, Name = DiscretionaryAccount.Name}},
                    new TransactionModel.Posting { Amount = -14m, Account = new TransactionModel.Account {Id = CreditCardAccount.Id, Name = CreditCardAccount.Name}},
                }
            },
        };

        [Test]
        public void ShouldGetTransactions()
        {
            var items = GetBuilderWithSetupTransactions()
                .ExecuteRequest(new ListTransactionsRequest())
                .Result
                .Items;

            TransactionModels.AssertEquals(items);
        }

        [Test]
        public void ShouldHandleSkip()
        {
            var items = GetBuilderWithSetupTransactions()
                .ExecuteRequest(new ListTransactionsRequest { Skip = 1 })
                .Result
                .Items;

            TransactionModels.Skip(1).AssertEquals(items);
        }

        [Test]
        public void ShouldHandleTake()
        {
            var items = GetBuilderWithSetupTransactions()
                .ExecuteRequest(new ListTransactionsRequest { Take = 1 })
                .Result
                .Items;

            TransactionModels.Take(1).AssertEquals(items);
        }

        [Test]
        public void ShouldOnlyReturnTransactionsForCorrectUser()
        {
            var items = GetBuilderWithSetupTransactions()
                .AsUser()
                .ExecuteRequest(new ListTransactionsRequest())
                .Result
                .Items;

            Enumerable.Empty<TransactionModel>().AssertEquals(items);
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