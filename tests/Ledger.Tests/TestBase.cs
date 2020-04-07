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
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables(prefix: "Ledger_")
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);

            Startup.ConfigureIoc(services);

            _serviceProvider = services.BuildServiceProvider();

            AsUserId(new Guid("da1ac815-ec39-4ca8-a8f3-7f9856384e32"));

            _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        [TearDown]
        public void Cleanup()
        {
            _transactionScope.Dispose();
        }

        protected void AsUserId(Guid userId)
        {
            var requestContext = GetInstance<IRequestContext>();
            requestContext.UserId = userId;
            requestContext.IsAuthenticated = true;
        }

        protected T GetInstance<T>() => _serviceProvider.GetRequiredService<T>();
    }
}