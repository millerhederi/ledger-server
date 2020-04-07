using System;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class GetTransactionServiceTests : TestBase
    {
        [Test]
        public async Task ShouldGetTransactionAsync()
        {
            var getTransactionService = GetInstance<IGetTransactionService>();
            var transaction = await getTransactionService
                .ExecuteAsync(new Guid("fdcb5bc6-e59f-4b1d-85f7-dc819f5b6c05"), CancellationToken.None);

            var expected = new TransactionModel
            {
                Description = "Groceries", 
                Amount = 24.87m, 
                PostedDate = new DateTime(2020, 3, 26)
            };

            AssertTransaction(expected, transaction);
        }

        [Test]
        public async Task ShouldHandleInvalidIdAsync()
        {
            var getTransactionService = GetInstance<IGetTransactionService>();
            var transaction = await getTransactionService.ExecuteAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.IsNull(transaction);
        }

        [Test]
        public async Task ShouldHandleGettingValidTransactionWithInvalidUserAsync()
        {
            AsUserId(Guid.NewGuid());

            var getTransactionService = GetInstance<IGetTransactionService>();
            var transaction = await getTransactionService
                .ExecuteAsync(new Guid("fdcb5bc6-e59f-4b1d-85f7-dc819f5b6c05"), CancellationToken.None);

            Assert.IsNull(transaction);
        }

        private static void AssertTransaction(TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
        }
    }
}