using System.Collections.Generic;
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
        Task<int> ExecuteAsync(CommandDefinition command);

        Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

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

        public async Task<int> ExecuteAsync(CommandDefinition command)
        {
            using (var cn = await CreateConnectionAsync(command.CancellationToken))
            {
                return await cn.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
        {
            using (var cn = await CreateConnectionAsync(command.CancellationToken))
            {
                return await cn.QueryAsync<T>(command);
            }
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition command)
        {
            using (var cn = await CreateConnectionAsync(command.CancellationToken))
            {
                return await cn.QuerySingleOrDefaultAsync<T>(command);
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
