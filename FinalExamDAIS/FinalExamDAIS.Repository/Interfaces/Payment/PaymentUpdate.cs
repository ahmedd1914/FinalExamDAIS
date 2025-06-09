using System.Data.SqlTypes;
using FinalExamDAIS.Models.Enums;

namespace FinalExamDAIS.Repository.Interfaces.Payment
{
    public class PaymentUpdate
    {
        public PaymentStatus Status { get; set; }
        public SqlDateTime? ProcessedDate { get; set; }
        public SqlString ErrorMessage { get; set; }
        public SqlString ProcessingDetails { get; set; }
    }
}