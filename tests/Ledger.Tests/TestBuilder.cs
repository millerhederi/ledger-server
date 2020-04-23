using System;
using System.Threading;
using Ledger.WebApi;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Models;
using Ledger.WebApi.Requests;
using MediatR;
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
            var builder = ExecuteRequest(new AddUserRequest {UserName = Guid.NewGuid().ToString(), Password = "Password123!"});
            builder.AsUser(builder.Result.Id);
            return WithResult(builder.Result.Id);
        }

        public TestBuilder<Guid> UpsertAccount(AccountModel model)
        {
            return WithResult(ExecuteRequest(new UpsertAccountRequest {Account = model}).Result.Id);
        }

        public TestBuilder<Guid> UpsertTransaction(TransactionModel model)
        {
            return WithResult(ExecuteRequest(new UpsertTransactionRequest {Transaction = model}).Result.Id);
        }

        public TestBuilder<TransactionModel> GetTransaction(Guid id)
        {
            return WithResult(ExecuteRequest(new GetTransactionRequest(id)).Result.Transaction);
        }

        public TestBuilder<TResponse> ExecuteRequest<TResponse>(IRequest<TResponse> request)
        {
            return WithResult(GetInstance<IMediator>().Send(request, CancellationToken.None).Result);
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