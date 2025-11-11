using Npgsql;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace FGT.Extensions
{
    /// <summary>
    /// Extensões para operações de bulk insert no PostgreSQL
    /// Usa COPY FROM STDIN para máxima performance
    /// </summary>
    public static class PostgresBulkExtensions
    {
        /// <summary>
        /// Executa bulk insert no PostgreSQL usando COPY FROM STDIN (método mais rápido)
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="context">Contexto do EF Core</param>
        /// <param name="entities">Lista de entidades para inserir</param>
        /// <param name="tableName">Nome da tabela</param>
        /// <param name="columnMapping">Mapeamento de propriedades para colunas (PropertyName -> ColumnName, ValueGetter)</param>
        /// <returns>Número de linhas inseridas</returns>
        public static async Task<long> BulkInsertAsync<T>(
            this DbContext context,
            IEnumerable<T> entities,
            string tableName,
            Dictionary<string, (string ColumnName, Func<T, object?> ValueGetter)> columnMapping) where T : class
        {
            var connection = context.Database.GetDbConnection() as NpgsqlConnection;
            if (connection == null)
            {
                throw new InvalidOperationException("BulkInsert só funciona com NpgsqlConnection (PostgreSQL)");
            }

            var wasOpen = connection.State == ConnectionState.Open;
            if (!wasOpen)
            {
                await connection.OpenAsync();
            }

            try
            {
                // Construir lista de colunas
                var columns = columnMapping.Values.Select(v => v.ColumnName).ToList();
                var copyCommand = $"COPY {tableName} ({string.Join(", ", columns)}) FROM STDIN (FORMAT BINARY)";

                long rowsInserted = 0;

                using (var writer = await connection.BeginBinaryImportAsync(copyCommand))
                {
                    foreach (var entity in entities)
                    {
                        await writer.StartRowAsync();

                        foreach (var mapping in columnMapping.Values)
                        {
                            var value = mapping.ValueGetter(entity);

                            if (value == null)
                            {
                                await writer.WriteNullAsync();
                            }
                            else
                            {
                                await writer.WriteAsync(value, GetNpgsqlDbType(value.GetType()));
                            }
                        }

                        rowsInserted++;
                    }

                    await writer.CompleteAsync();
                }

                return rowsInserted;
            }
            finally
            {
                if (!wasOpen && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        /// <summary>
        /// Mapeia tipos .NET para NpgsqlDbType
        /// </summary>
        private static NpgsqlTypes.NpgsqlDbType GetNpgsqlDbType(Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Int16 => NpgsqlTypes.NpgsqlDbType.Smallint,
                TypeCode.Int32 => NpgsqlTypes.NpgsqlDbType.Integer,
                TypeCode.Int64 => NpgsqlTypes.NpgsqlDbType.Bigint,
                TypeCode.Decimal => NpgsqlTypes.NpgsqlDbType.Numeric,
                TypeCode.Double => NpgsqlTypes.NpgsqlDbType.Double,
                TypeCode.String => NpgsqlTypes.NpgsqlDbType.Text,
                TypeCode.DateTime => NpgsqlTypes.NpgsqlDbType.Timestamp,
                TypeCode.Boolean => NpgsqlTypes.NpgsqlDbType.Boolean,
                _ => NpgsqlTypes.NpgsqlDbType.Text
            };
        }
    }
}
