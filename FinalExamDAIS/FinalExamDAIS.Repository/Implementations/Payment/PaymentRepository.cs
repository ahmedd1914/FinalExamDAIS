using Microsoft.Data.SqlClient;
using FinalExamDAIS.Repository.Base;
using FinalExamDAIS.Repository.Interfaces.Payment;
using FinalExamDAIS.Models;
using FinalExamDAIS.Repository.Helpers;
using FinalExamDAIS.Models.Enums;
using System.Runtime.CompilerServices;

namespace FinalExamDAIS.Repository.Implementations.Payment
{
    public class PaymentRepository : BaseRepository<Models.Payments, PaymentFilter, PaymentUpdate>, IPaymentRepository
    {
        private const string IdDbFieldName = "PaymentId";
        public PaymentRepository(string connectionString) : base(connectionString)
        {
        }

        protected override string GetTableName() => "Payments";

        protected override string[] GetColumns() => new[]
        {
            "PaymentId",
            "FromAccountId",
            "ToAccountNumber",
            "Amount",
            "Reason",
            "Status",
            "CreatedDate",
            "ProcessedDate",
            "CreatedByUserId"
        };

        protected override Models.Payments MapEntity(SqlDataReader reader)
        {
            return new Models.Payments
            {
                PaymentId = Convert.ToInt32(reader["PaymentId"]),
                FromAccountId = Convert.ToInt32(reader["FromAccountId"]),
                ToAccountNumber = Convert.ToString(reader["ToAccountNumber"]),
                Amount = Convert.ToDecimal(reader["Amount"]),
                Reason = Convert.ToString(reader["Reason"]),
                Status = Convert.ToString(reader["Status"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                ProcessedDate = reader["ProcessedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ProcessedDate"]) : null,
                CreatedByUserId = Convert.ToInt32(reader["CreatedByUserId"])
            };
        }

        public override async Task<int> CreateAsync(Models.Payments entity)
        {
            return await base.CreateEntityAsync(entity, IdDbFieldName);
        }

        public override async Task<Models.Payments> RetrieveAsync(int objectId)
        {
            return await base.RetrieveEntityAsync(IdDbFieldName, objectId);
        }

        public override async IAsyncEnumerable<Models.Payments> RetrieveCollectionAsync(PaymentFilter filter)
        {
            var query = "SELECT * FROM Payments WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (filter.FromAccountId.HasValue)
            {
                query += " AND FromAccountId = @FromAccountId";
                parameters.Add(SqlQueryHelper.CreateParameter("@FromAccountId", filter.FromAccountId.Value));
            }

            if (filter.ToAccountNumber != null)
            {
                query += " AND ToAccountNumber = @ToAccountNumber";
                parameters.Add(SqlQueryHelper.CreateParameter("@ToAccountNumber", filter.ToAccountNumber));
            }

            if (filter.Status != null)
            {
                query += " AND Status = @Status";
                parameters.Add(SqlQueryHelper.CreateParameter("@Status", filter.Status));
            }

            if (filter.UserId.HasValue)
            {
                query += " AND CreatedByUserId = @UserId";
                parameters.Add(SqlQueryHelper.CreateParameter("@UserId", filter.UserId.Value));
            }

            using var reader = await SqlQueryHelper.ExecuteReaderAsync(query, parameters.ToArray(), _connectionString);
            while (await reader.ReadAsync())
            {
                yield return MapEntity(reader);
            }
        }

        public override async Task<bool> UpdateAsync(int objectId, PaymentUpdate update)
        {
            UpdateCommand updateCommand = new UpdateCommand(GetTableName(), IdDbFieldName, objectId, _connectionString);

            updateCommand.AddUpdate("Status", update.Status.ToString());
            if (update.ProcessedDate.HasValue)
            {
                updateCommand.AddUpdate("ProcessedDate", update.ProcessedDate.Value);
            }
            
            return await updateCommand.ExecuteAsync();
        }

        public override async Task<bool> DeleteAsync(int objectId)
        {
            return await base.DeleteEntityAsync(IdDbFieldName, objectId);
        }

        public async Task<IEnumerable<Payments>> GetPaymentsByUserIdAsync(int userId)
        {
            var filter = new PaymentFilter { UserId = userId };
            var payments = new List<Payments>();
            await foreach (var payment in RetrieveCollectionAsync(filter))
            {
                payments.Add(payment);
            }
            return payments;
        }

        public async Task<IEnumerable<Payments>> GetPendingPaymentsAsync()
        {
            var filter = new PaymentFilter { Status = PaymentStatus.Pending };
            var payments = new List<Payments>();
            await foreach (var payment in RetrieveCollectionAsync(filter))
            {
                payments.Add(payment);
            }
            return payments;
        }


    }
}   