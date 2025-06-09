using System.Data.SqlTypes;
using FinalExamDAIS.Repository.Helpers;
using FinalExamDAIS.Models.Enums;

namespace FinalExamDAIS.Repository.Interfaces.Payment
{
    public class PaymentFilter
    {
        public SqlInt32? FromAccountId { get; set; }
        public SqlString ToAccountNumber { get; set; }
        public PaymentStatus? Status { get; set; }
        public SqlInt32? UserId { get; set; }
        public SqlDateTime? FromDate { get; set; }
        public SqlDateTime? ToDate { get; set; }
        public SqlDecimal? MinAmount { get; set; }
        public SqlDecimal? MaxAmount { get; set; }

        public Filter ToFilter()
        {
            var filter = new Filter();
            if (FromAccountId.HasValue)
                filter.AddCondition("FromAccountId", FromAccountId.Value);
            if (ToAccountNumber != null)
                filter.AddCondition("ToAccountNumber", ToAccountNumber);
            if (Status.HasValue)
                filter.AddCondition("Status", (int)Status.Value);
            if (UserId.HasValue)
                filter.AddCondition("UserId", UserId.Value);
            if (FromDate.HasValue)
                filter.AddCondition("CreatedDate", FromDate.Value);
            if (ToDate.HasValue)
                filter.AddCondition("CreatedDate", ToDate.Value);
            if (MinAmount.HasValue)
                filter.AddCondition("Amount", MinAmount.Value);
            return filter;
        }
    }
}