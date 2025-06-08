using System;
using System.Data;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using FinalExamDAIS.Models;
using FinalExamDAIS.Repository.Base;
using FinalExamDAIS.Repository.Helpers;
using FinalExamDAIS.Repository.Interfaces.User;

namespace FinalExamDAIS.Repository.Implementations.User
{
    public class UserRepository : BaseRepository<Models.User, UserFilter, UserUpdate>, IUserRepository
    {
        private const string IdDbFieldName = "UserId";

        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        protected override string GetTableName() => "Users";

        protected override string[] GetColumns() => new[]
        {
            IdDbFieldName,
            "Username",
            "Email",
            "PasswordHash",
            "FirstName",
            "LastName",
            "DateOfBirth",
            "CreatedAt",
            "IsActive"
        };

        protected override Models.User MapEntity(SqlDataReader reader)
        {
            return new Models.User
            {
                UserId = Convert.ToInt32(reader[IdDbFieldName]),
                Username = Convert.ToString(reader["Username"]),
                Email = Convert.ToString(reader["Email"]),
                PasswordHash = Convert.ToString(reader["PasswordHash"]),
                FirstName = Convert.ToString(reader["FirstName"]),
                LastName = Convert.ToString(reader["LastName"]),
                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

        public override async Task<int> CreateAsync(Models.User entity)
        {
            return await base.CreateEntityAsync(entity, IdDbFieldName);
        }

        public override async Task<Models.User> RetrieveAsync(int objectId)
        {
            return await base.RetrieveEntityAsync(IdDbFieldName, objectId);
        }

        public override async IAsyncEnumerable<Models.User> RetrieveCollectionAsync(UserFilter filter)
        {
            var users = await base.RetrieveEntitiesAsync(filter?.ToFilter());
            foreach (var user in users)
            {
                yield return user;
            }
        }

        public override async Task<bool> UpdateAsync(int objectId, UserUpdate update)
        {
            var command = update.ToUpdateCommand(GetTableName(), objectId, _connectionString);
            return await command.ExecuteAsync();
        }

        public override async Task<bool> DeleteAsync(int objectId)
        {
            return await base.DeleteEntityAsync(IdDbFieldName, objectId);
        }

        public async Task<Models.User> GetByEmailAsync(string email)
        {
            var filter = new Filter().AddCondition("Email", email);
            var users = await RetrieveEntitiesAsync(filter);
            return users.FirstOrDefault();
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            var update = new UserUpdate { Password = new SqlString(newPasswordHash) };
            return await UpdateAsync(userId, update);
        }
    }
}
