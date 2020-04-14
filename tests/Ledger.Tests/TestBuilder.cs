using System;
using System.Collections.Generic;
using System.Threading;
using Ledger.WebApi;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;
using Ledger.WebApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Ledger.Tests
{
    // TODO: explore https://stackoverflow.com/a/25308545 to avoid having to block on the tasks
    public class TestBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        private TestBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected TestBuilder(TestBuilder builder) : this(builder._serviceProvider)
        {
        }

        public static TestBuilder Begin()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables(prefix: "Ledger_")
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);

            Startup.ConfigureIoc(services);

            var serviceProvider = services.BuildServiceProvider();

            return new TestBuilder(serviceProvider)
                .AsUser(new Guid("da1ac815-ec39-4ca8-a8f3-7f9856384e32"));
        }

        public TestBuilder<TResult> WithResult<TResult>(TResult result)
        {
            return new TestBuilder<TResult>(result, this);
        }

        public TestBuilder AsUser(Guid userId)
        {
            var requestContext = GetInstance<IRequestContext>();
            requestContext.UserId = userId;
            requestContext.IsAuthenticated = true;

            return this;
        }

        public TestBuilder<Guid> AsUser()
        {
            var builder = AddUser(new LoginModel {UserName = Guid.NewGuid().ToString(), Password = "Password123!"});
            builder.AsUser(builder.Result);
            return builder;
        }

        public TestBuilder<Guid> AddUser(LoginModel model)
        {
            return WithResult(GetInstance<IAddUserService>().ExecuteAsync(model, CancellationToken.None).Result);
        }

        public TestBuilder<ICollection<AccountModel>> ListAccounts()
        {
            return WithResult(GetInstance<IListAccountsService>().ExecuteAsync(CancellationToken.None).Result);
        }

        public TestBuilder<Guid> UpsertTransaction(TransactionModel model)
        {
            return WithResult(GetInstance<IUpsertTransactionService>().ExecuteAsync(model, CancellationToken.None).Result);
        }

        public TestBuilder<TransactionModel> GetTransaction(Guid id)
        {
            return WithResult(GetInstance<IGetTransactionService>().ExecuteAsync(id, CancellationToken.None).Result);
        }

        public TestBuilder<ICollection<TransactionModel>> ListTransactions(int skip, int take)
        {
            return WithResult(GetInstance<IListTransactionsService>().ExecuteAsync(skip, take, CancellationToken.None).Result);
        }

        public TestBuilder<ICollection<TransactionModel>> ListTransactions()
        {
            return WithResult(GetInstance<IListTransactionsService>().ExecuteAsync(CancellationToken.None).Result);
        }

        public T GetInstance<T>() => _serviceProvider.GetRequiredService<T>();
    }

    public class TestBuilder<TResult> : TestBuilder
    {
        public TestBuilder(TResult result, TestBuilder builder) : base(builder)
        {
            Result = result;
        }

        public TResult Result { get; }
    }
}