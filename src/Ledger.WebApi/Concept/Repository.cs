using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Ledger.WebApi.Concept
{
    public interface IRepository
    {
        Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command);
    }

    public class Repository : IRepository
    {
        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection");

        public async Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command)
        {
            using (var cn = await CreateConnectionAsync(command.CancellationToken).ConfigureAwait(false))
            {
                return await cn.QuerySingleOrDefaultAsync<T>(command).ConfigureAwait(false);
            }
        }

        private async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            var cn = new SqlConnection(ConnectionString);

            try
            {
                await cn.OpenAsync(cancellationToken);
            }
            catch
            {
                await cn.CloseAsync();
                throw;
            }

            return cn;
        }
    }
}
