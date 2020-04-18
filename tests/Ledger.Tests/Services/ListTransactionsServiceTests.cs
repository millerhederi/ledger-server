using System;
using System.Linq;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class ListTransactionsServiceTests : TestBase
    {
        private static readonly TransactionModel[] TransactionModels =
        {
            new TransactionModel {Description = "Groceries", Amount = 24.87m, PostedDate = new DateTime(2020, 3, 26)},
            new TransactionModel {Description = "Netflix", Amount = 13.00m, PostedDate = new DateTime(2020, 3, 28)},
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

            foreach (var transaction in TransactionModels)
            {
                builder.UpsertTransaction(transaction);
            }

            return builder;
        }
    }
}