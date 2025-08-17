using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<List<T>> ExecuteStoredProcedureAsync<T>(this DbContext context, string storedProcedureName, params SqlParameter[] parameters) where T : class, new()
        {
            var connection = context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();

            var result = new List<T>();
            await using var reader = await command.ExecuteReaderAsync();
            var properties = typeof(T).GetProperties();

            while (await reader.ReadAsync())
            {
                var instance = new T();
                foreach (var property in properties)
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        property.SetValue(instance, reader.GetValue(reader.GetOrdinal(property.Name)));
                    }
                }
                result.Add(instance);
            }

            await connection.CloseAsync();
            return result;
        }

        public static async Task<(List<TParent> Parents, List<TChild> Children)> ExecuteStoredProcedureWithTwoDatasetsAsync<TParent, TChild>(this DbContext context, string storedProcedureName, params SqlParameter[] parameters)
            where TParent : class, new()
            where TChild : class, new()
        {
            var connection = context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();

            var parents = new List<TParent>();
            var children = new List<TChild>();

            await using var reader = await command.ExecuteReaderAsync();
            var parentProperties = typeof(TParent).GetProperties();
            var childProperties = typeof(TChild).GetProperties();

            // Read parent dataset
            while (await reader.ReadAsync())
            {
                var parentInstance = new TParent();
                foreach (var property in parentProperties)
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        property.SetValue(parentInstance, reader.GetValue(reader.GetOrdinal(property.Name)));
                    }
                }
                parents.Add(parentInstance);
            }

            // Move to the next result set (children dataset)
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var childInstance = new TChild();
                    foreach (var property in childProperties)
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            property.SetValue(childInstance, reader.GetValue(reader.GetOrdinal(property.Name)));
                        }
                    }
                    children.Add(childInstance);
                }
            }

            await connection.CloseAsync();
            return (parents, children);
        }

        public static async Task<List<List<object>>> ExecuteStoredProcedureWithMultipleDatasetsAsync(this DbContext context, string storedProcedureName, params SqlParameter[] parameters)
        {
            var connection = context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();

            var resultSets = new List<List<object>>();

            await using var reader = await command.ExecuteReaderAsync();

            do
            {
                var resultSet = new List<object>();
                var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    foreach (var column in columns)
                    {
                        row[column] = reader[column];
                    }
                    resultSet.Add(row);
                }

                resultSets.Add(resultSet);
            } while (await reader.NextResultAsync());

            await connection.CloseAsync();
            return resultSets;
        }

        public static async Task<int> ExecuteStoredProcedureNonQueryAsync(this DbContext context, string storedProcedureName, params SqlParameter[] parameters)
        {
            var connection = context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            return result;
        }
    }
}
