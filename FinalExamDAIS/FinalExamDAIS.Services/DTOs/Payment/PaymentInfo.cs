using FinalExamDAIS.Models.Enums;
using FinalExamDAIS.Models;

namespace FinalExamDAIS.Services.DTOs.Payment
{
    public class PaymentInfo : BaseResponse
    {
        public int PaymentId { get; set; }
        public int FromAccountId { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public int CreatedByUserId { get; set; }

        public PaymentInfo()
        {
            Status = PaymentStatus.Pending.ToString();
        }

       
    }
}