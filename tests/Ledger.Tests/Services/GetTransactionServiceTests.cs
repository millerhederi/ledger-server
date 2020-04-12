using System;
using System.Threading.Tasks;
using Ledger.WebApi.Models;
using NUnit.Framework;

namespace Ledger.Tests.Services
{
    public class GetTransactionServiceTests : TestBase
    {
        [Test]
        public void ShouldGetTransaction()
        {
            var builder = TestBuilder.Begin()
                .GetTransaction(new Guid("fdcb5bc6-e59f-4b1d-85f7-dc819f5b6c05"));

            var expected = new TransactionModel
            {
                Description = "Groceries", 
                Amount = 24.87m, 
                PostedDate = new DateTime(2020, 3, 26)
            };

            AssertTransaction(expected, builder.Result);
        }

        [Test]
        public void ShouldHandleInvalidId()
        {
            var builder = TestBuilder.Begin()
                .GetTransaction(Guid.NewGuid());

            Assert.IsNull(builder.Result);
        }

        [Test]
        public void ShouldHandleGettingValidTransactionWithInvalidUser()
        {
            var builder = TestBuilder.Begin()
                .AsUser(Guid.NewGuid())
                .GetTransaction(new Guid("fdcb5bc6-e59f-4b1d-85f7-dc819f5b6c05"));

            Assert.IsNull(builder.Result);
        }

        private static void AssertTransaction(TransactionModel expected, TransactionModel actual)
        {
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.PostedDate, actual.PostedDate);
        }
    }
}