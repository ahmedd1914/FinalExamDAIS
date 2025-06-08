using System.Data.SqlTypes;
using FinalExamDAIS.Repository.Helpers;

namespace FinalExamDAIS.Repository.Interfaces.User
{
    public class UserUpdate
    {
        public SqlString? FirstName { get; set; }
        public SqlString? LastName { get; set; }
        public SqlString? Password { get; set; }
        public SqlString? Email { get; set; }

        public UpdateCommand ToUpdateCommand(string tableName, int userId, string connectionString)
        {
            var command = new UpdateCommand(tableName, "UserId", userId, connectionString);

            if (FirstName.HasValue && !FirstName.Value.IsNull)
                command.AddUpdate("FirstName", FirstName.Value.Value);

            if (LastName.HasValue && !LastName.Value.IsNull)
                command.AddUpdate("LastName", LastName.Value.Value);

            if (Password.HasValue && !Password.Value.IsNull)
                command.AddUpdate("Password", Password.Value.Value);

            if (Email.HasValue && !Email.Value.IsNull)
                command.AddUpdate("Email", Email.Value.Value);

            return command;
        }
    }
}
