using System;
using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;
using NUnit.Framework;

namespace Ledger.Tests.Requests
{
    public class ListPostingsRequestTests : TestBase
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
                Id = Guid.NewGuid(),
                Description = "Groceries",
                PostedDate = new DateTime(2020, 3, 26),
                Postings =
                {
                    new TransactionModel.Posting { Amount = 87.34m, Account = new TransactionModel.Account {Id = FoodAccount.Id, Name = FoodAccount.Name}},
                    new TransactionModel.Posting { Amount = -87.34m, Account = new TransactionModel.Account {Id = CreditCardAccount.Id, Name = CreditCardAccount.Name}}
                }
            },
            new TransactionModel
            {
                Id = Guid.NewGuid(),
                Description = "Pizza",
                PostedDate = new DateTime(2020, 3, 28),
                Postings =
                {
                    new TransactionModel.Posting { Amount = 14m, Account = new TransactionModel.Account {Id = FoodAccount.Id, Name = FoodAccount.Name} },
                    new TransactionModel.Posting { Amount = -14m, Account = new TransactionModel.Account {Id = CreditCardAccount.Id, Name = CreditCardAccount.Name} }
                }
            },
        };

        [Test]
        public void ShouldListPostings()
        {
            var response = GetBuilderWithSetupTransactions()
                .ExecuteRequest(new ListPostingsRequest(FoodAccount.Id))
                .Result;

            var expectedPostings = new []
            {
                new ListPostingsResponse.Posting
                {
                    Amount = 14m,
                    PostedDate = TransactionModels[1].PostedDate,
                    Description = TransactionModels[1].Description,
                    TransactionId = TransactionModels[1].Id,
                },
                new ListPostingsResponse.Posting
                {
                    Amount = 87.34m,
                    PostedDate = TransactionModels[0].PostedDate,
                    Description = TransactionModels[0].Description,
                    TransactionId = TransactionModels[0].Id,
                },
            };

            expectedPostings.AssertEquals(response.Items);
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