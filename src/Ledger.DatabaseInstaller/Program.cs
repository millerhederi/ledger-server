using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace Ledger.DatabaseInstaller
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(prefix: "Ledger_")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.Contains("Scripts.Script_"))
                .LogToConsole()
                .Build();

            upgradeEngine.PerformUpgrade();
        }
    }
}