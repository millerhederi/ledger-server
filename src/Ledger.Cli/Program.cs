using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ledger.WebApi;
using Ledger.WebApi.Concept;
using Ledger.WebApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ledger.Cli
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;

        private static async Task Main(string[] args)
        {
            ConfigureIoc();
            SetRequestContext();

            var reader = new JournalReader();
            var transactions = await reader.ReadFromFileAsync(args[0]);

            var accounts = transactions
                .SelectMany(t => t.Postings.Select(p => p.Account))
                .Distinct()
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var account in accounts)
            {
                account.Id = await GetInstance<IUpsertAccountService>().ExecuteAsync(account, CancellationToken.None);
            }

            foreach (var transaction in transactions)
            {
                await GetInstance<IUpsertTransactionService>().ExecuteAsync(transaction, CancellationToken.None);
            }
        }

        private static T GetInstance<T>() => _serviceProvider.GetRequiredService<T>();

        private static void SetRequestContext()
        {
            var requestContext = GetInstance<IRequestContext>();
            requestContext.UserId = new Guid("55BE4AB8-D4EB-4B1D-9B37-397CA9D2FEF9");
            requestContext.IsAuthenticated = true;
        }

        private static void ConfigureIoc()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);

            Startup.ConfigureIoc(services);

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
