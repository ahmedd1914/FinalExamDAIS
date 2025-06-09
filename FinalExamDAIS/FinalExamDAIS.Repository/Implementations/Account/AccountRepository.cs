using FinalExamDAIS.Repository.Base;
using FinalExamDAIS.Repository.Interfaces.Account;
using FinalExamDAIS.Repository.Helpers;
using Microsoft.Data.SqlClient;

namespace FinalExamDAIS.Repository.Implementations.Account
{
    public class AccountRepository : BaseRepository<Models.Account, AccountFilter, AccountUpdate>, IAccountRepository
    {
        private const string IdDbFieldName = "AccountId";
        public AccountRepository(string connectionString) : base(connectionString)
        {
        }

        protected override string GetTableName() => "Accounts";

        protected override string[] GetColumns() => new[]
        {
            "AccountId",
            "AccountNumber",
            "AvailableAmount",
            "IsActive"
        };

        protected override Models.Account MapEntity(SqlDataReader reader)
        {
            return new Models.Account
            {
                AccountId = Convert.ToInt32(reader["AccountId"]),
                AccountNumber = Convert.ToString(reader["AccountNumber"]),
                AvailableAmount = Convert.ToDecimal(reader["AvailableAmount"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

        public override Task<int> CreateAsync(Models.Account entity)
        {
            throw new NotImplementedException();
        }

        public override async Task<Models.Account> RetrieveAsync(int objectId)
        {
            return await base.RetrieveEntityAsync(IdDbFieldName, objectId);
        }

        public override async IAsyncEnumerable<Models.Account> RetrieveCollectionAsync(AccountFilter filter)
        {
            var accounts = await base.RetrieveEntitiesAsync(filter?.ToFilter());
            foreach (var account in accounts)
            {
                yield return account;
            }
        }

        public override async Task<bool> UpdateAsync(int objectId, AccountUpdate update)
        {
            try
            {
                var updates = new Dictionary<string, object>
                {
                    { "AvailableAmount", update.AvailableAmount }
                };

                var setClause = SqlQueryHelper.BuildSetClause(updates);
                var query = SqlQueryHelper.BuildUpdateQuery(GetTableName(), setClause, IdDbFieldName);
                var parameters = new[]
                {
                    SqlQueryHelper.CreateParameter("AvailableAmount", update.AvailableAmount),
                    SqlQueryHelper.CreateParameter(IdDbFieldName, objectId)
                };

                var rowsAffected = await SqlQueryHelper.ExecuteNonQueryAsync(query, parameters, _connectionString);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating account {objectId}", ex);
            }
        }

        public override Task<bool> DeleteAsync(int objectId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Models.Account>> GetAccountsByUserIdAsync(int userId)
        {
            var query = @"
                SELECT a.AccountID, a.AccountNumber, a.AvailableAmount, a.IsActive
                FROM Accounts a
                INNER JOIN UserAccounts ua ON a.AccountID = ua.AccountID
                WHERE ua.UserID = @UserID AND a.IsActive = 1";

            var parameters = new[] { SqlQueryHelper.CreateParameter("@UserID", userId) };
            var accounts = new List<Models.Account>();

            using var reader = await SqlQueryHelper.ExecuteReaderAsync(query, parameters, _connectionString);
            while (await reader.ReadAsync())
            {
                accounts.Add(MapEntity(reader));
            }

            return accounts;
        }

        public async Task<Models.Account> GetAccountByNumberAsync(string accountNumber)
        {
            var query = @"
                SELECT AccountId, AccountNumber, AvailableAmount, IsActive
                FROM Accounts
                WHERE AccountNumber = @AccountNumber";

            var parameters = new[] { SqlQueryHelper.CreateParameter("@AccountNumber", accountNumber) };

            using var reader = await SqlQueryHelper.ExecuteReaderAsync(query, parameters, _connectionString);
            if (await reader.ReadAsync())
            {
                return MapEntity(reader);
            }

            return null;
        }
    }
}