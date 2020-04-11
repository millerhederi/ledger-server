using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Ledger.WebApi.Concept
{
    public class EntitySchemaInfo
    {
        public const string TempTableName = "#temp";
        private const string SchemaName = "dbo";

        private EntitySchemaInfo(string tableName, ICollection<ColumnDto> columns, ICollection<PropertyInfo> propertyInfos)
        {
            PropertyInfos = propertyInfos;
            CreateTempTableSql = $"select * into {TempTableName} from {GetBracketedName(tableName)} where 1 = 0";
            MergeSql = BuildMergeStatement(tableName, columns);
        }

        public ICollection<PropertyInfo> PropertyInfos { get; }

        public string CreateTempTableSql { get; }

        public string DropTempTableSql => $"drop table {TempTableName}";

        public string MergeSql { get; }

        public static async Task<EntitySchemaInfo> LoadAsync<T>(
            string tableName,
            IRepository repository, 
            CancellationToken cancellationToken)
        {
            var columns = await LoadColumnsAsync(tableName, repository, cancellationToken);
            var propertyInfos = typeof(T).GetProperties();

            return new EntitySchemaInfo(tableName, columns.ToList(), propertyInfos);
        }

        private static string GetBracketedName(string name) => $"[{name}]";

        private static async Task<IEnumerable<ColumnDto>> LoadColumnsAsync(
            string tableName,
            IRepository repository,
            CancellationToken cancellationToken)
        {
            const string sql = @"
select
     c.column_id as [ColumnId]
	,c.name as [ColumnName]
	,ic.key_ordinal as [PrimaryKeyOrdinal]
from sys.tables t
	inner join sys.columns c on c.object_id = t.object_id
	left outer join sys.indexes i on i.object_id = t.object_id and i.is_primary_key = 1
	left outer join sys.index_columns ic on ic.object_id = t.object_id and ic.column_id = c.column_id
where t.name = @TableName and schema_name(t.schema_id) = @SchemaName";

            var parameters = new DynamicParameters();
            parameters.Add("TableName", tableName);
            parameters.Add("SchemaName", SchemaName);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            var columns = (await repository.QueryAsync<ColumnDto>(command)).ToList();

            if (columns.Count == 0)
            {
                throw new InvalidEnumArgumentException($"Unable to find a table '{tableName}' for schema '{SchemaName}'.");
            }

            return columns;
        }

        private string BuildMergeStatement(string tableName, ICollection<ColumnDto> columns)
        {
            var allColumns = columns
                .OrderBy(c => c.ColumnId)
                .Select(c => GetBracketedName(c.ColumnName))
                .ToList();

            var primaryKeyColumns = columns
                .Where(c => c.PrimaryKeyOrdinal.HasValue)
                .OrderBy(c => c.PrimaryKeyOrdinal.Value)
                .Select(c => GetBracketedName(c.ColumnName))
                .ToList();

            if (primaryKeyColumns.Count == 0)
            {
                throw new InvalidOperationException($"Unable to find a primary key for the table '{tableName}'.");
            }

            var updatableColumns = columns
                .Where(c => !c.PrimaryKeyOrdinal.HasValue)
                .OrderBy(c => c.ColumnId)
                .Select(c => GetBracketedName(c.ColumnName))
                .ToList();

            if (updatableColumns.Count == 0)
            {
                throw new InvalidOperationException($"Unable to find columns that can be updated for the table '{tableName}'.");
            }

            var join = string.Join("and", primaryKeyColumns.Select(c => $"target.{c} = source.{c}"));
            var update = string.Join(", ", updatableColumns.Select(c => $"target.{c} = source.{c}"));
            var insertColumns = string.Join(", ", allColumns);
            var insertValues = string.Join(", ", allColumns.Select(c => $"source.{c}"));

            return @$"
merge into {GetBracketedName(tableName)} as target
using {TempTableName} as source on {join}
when matched then
    update set {update}
when not matched then
    insert ({insertColumns})
    values ({insertValues});";
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class ColumnDto 
        {
            public string ColumnName { get; set; }
            public int? PrimaryKeyOrdinal { get; set; }
            public int ColumnId { get; set; }
        }
    }
}