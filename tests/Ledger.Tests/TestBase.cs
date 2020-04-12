using System;
using System.Transactions;
using Ledger.WebApi;
using Ledger.WebApi.Concept;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ledger.Tests
{
    public class TestBase
    {
        private TransactionScope _transactionScope;

        [SetUp]
        public void Setup()
        {
            _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        [TearDown]
        public void Cleanup()
        {
            _transactionScope.Dispose();
        }
    }
}