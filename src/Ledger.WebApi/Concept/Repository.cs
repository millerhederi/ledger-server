using System;
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
        
        Task UpsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : IEntity;

        Task UpsertAsync<T>(T entity, CancellationToken cancellationToken) where T : IEntity;
    }

    public class Repository : IRepository
    {
        private readonly EntitySchemaInfoCache _entitySchemaInfoCache;
        private readonly IConfiguration _configuration;

        public Repository(IConfiguration configuration)
        {
            _configuration = configuration;

            _entitySchemaInfoCache = new EntitySchemaInfoCache(this);
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

        public async Task UpsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken)
            where T : IEntity
        {
            var entitySchemaInfo = await GetEntitySchemaInfoAsync<T>(cancellationToken);

            using (var cn = (SqlConnection)await CreateConnectionAsync(cancellationToken))
            {
                await ExecuteCommandAsync(entitySchemaInfo.CreateTempTableSql, cn);

                try
                {
                    await BulkCopyAsync(cn);
                    await ExecuteCommandAsync(entitySchemaInfo.MergeSql, cn);
                }
                finally
                {
                    await ExecuteCommandAsync(entitySchemaInfo.DropTempTableSql, cn);
                }
            }

            async Task ExecuteCommandAsync(string sql, SqlConnection cn)
            {
                using (var command = new SqlCommand(sql, cn))
                {
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            async Task BulkCopyAsync(SqlConnection cn)
            {
                using (var dataTable = GetDataTable())
                {
                    using (var bulkCopy = new SqlBulkCopy(cn, SqlBulkCopyOptions.CheckConstraints, null))
                    {
                        bulkCopy.DestinationTableName = EntitySchemaInfo.TempTableName;

                        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                    }
                }
            }

            DataTable GetDataTable()
            {
                var dataColumns = GetDataColumns();
                var dataTable = new DataTable();

                dataTable.Columns.AddRange(dataColumns);

                var rowValues = new object[dataColumns.Length];
                foreach (var entity in entities)
                {
                    var i = 0;
                    
                    foreach (var propertyInfo in entitySchemaInfo.PropertyInfos)
                    {
                        rowValues[i] = propertyInfo.GetValue(entity);
                        i++;
                    }

                    dataTable.Rows.Add(rowValues);
                }

                return dataTable;
            }

            DataColumn[] GetDataColumns()
            {
                var columns = new List<DataColumn>();

                foreach (var propertyInfo in entitySchemaInfo.PropertyInfos)
                {
                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    columns.Add(new DataColumn(propertyInfo.Name, propertyType));
                }

                return columns.ToArray();
            }
        }

        public Task UpsertAsync<T>(T entity, CancellationToken cancellationToken) where T : IEntity
        {
            return UpsertAsync(new[] {entity}, cancellationToken);
        }

        private async Task<EntitySchemaInfo> GetEntitySchemaInfoAsync<T>(CancellationToken cancellationToken)
        {
            return await _entitySchemaInfoCache.GetAsync<T>(cancellationToken);
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

        private class EntitySchemaInfoCache
        {
            private readonly IRepository _repository;
            private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
            private readonly IDictionary<Type, EntitySchemaInfo> _cache = new Dictionary<Type, EntitySchemaInfo>();

            public EntitySchemaInfoCache(IRepository repository)
            {
                _repository = repository;
            }

            public async Task<EntitySchemaInfo> GetAsync<T>(CancellationToken cancellationToken)
            {
                if (_cache.TryGetValue(typeof(T), out var entitySchemaInfo))
                {
                    return entitySchemaInfo;
                }

                await _semaphoreSlim.WaitAsync(cancellationToken);
                try
                {
                    if (_cache.TryGetValue(typeof(T), out entitySchemaInfo))
                    {
                        return entitySchemaInfo;
                    }

                    entitySchemaInfo = await EntitySchemaInfo.LoadAsync<T>(typeof(T).Name, _repository, cancellationToken);
                    _cache.Add(typeof(T), entitySchemaInfo);
                    
                    return entitySchemaInfo;
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }
    }
}
