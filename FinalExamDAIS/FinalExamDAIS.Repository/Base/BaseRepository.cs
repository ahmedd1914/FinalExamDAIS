using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using FinalExamDAIS.Repository.Helpers;

namespace FinalExamDAIS.Repository.Base
{
    public abstract class BaseRepository<TObj, TFilter, TUpdate> : IBaseRepository<TObj, TFilter, TUpdate>
        where TObj : class
    {
        protected readonly string _connectionString;

        protected BaseRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected abstract string GetTableName();
        protected abstract string[] GetColumns();
        protected abstract TObj MapEntity(SqlDataReader reader);
        protected virtual string GetIdColumnName() => "Id";

        protected async Task<SqlConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public abstract Task<int> CreateAsync(TObj entity);
        public abstract Task<TObj> RetrieveAsync(int objectId);
        public abstract IAsyncEnumerable<TObj> RetrieveCollectionAsync(TFilter filter);
        public abstract Task<bool> UpdateAsync(int objectId, TUpdate update);
        public abstract Task<bool> DeleteAsync(int objectId);

        // Helper methods for derived classes
        protected async Task<int> CreateEntityAsync(TObj entity, string idDbFieldEnumeratorName = null)
        {
            try
            {
                var properties = typeof(TObj).GetProperties()
                    .Where(p => p.Name != idDbFieldEnumeratorName)
                    .ToList();

                string columns = string.Join(", ", properties.Select(p => p.Name));
                string parameters = string.Join(", ", properties.Select(p => "@" + p.Name));

                var query = SqlQueryHelper.BuildInsertQuery(GetTableName(), columns, parameters);
                var sqlParams = properties.Select(p => 
                    SqlQueryHelper.CreateParameter(p.Name, p.GetValue(entity))).ToArray();

                return await SqlQueryHelper.ExecuteScalarAsync<int>(query, sqlParams, _connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating entity in {GetTableName()}", ex);
            }
        }

        protected async Task<TObj> RetrieveEntityAsync(string idDbFieldName, int idDbFieldValue)
        {
            try
            {
                var columns = string.Join(", ", GetColumns());
                var query = SqlQueryHelper.BuildSelectByIdQuery(columns, GetTableName(), idDbFieldName);
                var parameter = SqlQueryHelper.CreateParameter(idDbFieldName, idDbFieldValue);

                using var reader = await SqlQueryHelper.ExecuteReaderAsync(query, new[] { parameter }, _connectionString);
                
                if (!reader.Read())
                {
                    return default;
                }

                var result = MapEntity(reader);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving entity from {GetTableName()}", ex);
            }
        }

        protected async Task<List<TObj>> RetrieveEntitiesAsync(Filter filter = null)
        {
            try
            {
                var columns = string.Join(", ", GetColumns());
                var query = SqlQueryHelper.BuildSelectAllQuery(columns, GetTableName());
                var parameters = new List<SqlParameter>();

                if (filter?.Conditions?.Any() == true)
                {
                    var conditions = filter.Conditions.Select(c => $"{c.Key} = @{c.Key}");
                    query += SqlQueryHelper.BuildWhereClause(string.Join(SqlQueryHelper.And, conditions));
                    parameters.AddRange(filter.Conditions.Select(c => 
                        SqlQueryHelper.CreateParameter(c.Key, c.Value)));
                }

                var results = new List<TObj>();
                using var reader = await SqlQueryHelper.ExecuteReaderAsync(query, parameters.ToArray(), _connectionString);
                while (await reader.ReadAsync())
                {
                    results.Add(MapEntity(reader));
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving collection from {GetTableName()}", ex);
            }
        }

        protected async Task<bool> DeleteEntityAsync(string idDbFieldName, int idDbFieldValue)
        {
            using var connection = await CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var query = SqlQueryHelper.BuildDeleteQuery(GetTableName(), idDbFieldName);
                var parameter = SqlQueryHelper.CreateParameter(idDbFieldName, idDbFieldValue);

                var rowsAffected = await SqlQueryHelper.ExecuteNonQueryAsync(query, new[] { parameter }, _connectionString);
                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Error deleting entity from {GetTableName()}", ex);
            }
        }
    }
}
