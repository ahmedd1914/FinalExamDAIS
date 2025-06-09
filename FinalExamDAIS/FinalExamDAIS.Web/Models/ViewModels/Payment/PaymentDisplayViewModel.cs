using System;

namespace FinalExamDAIS.Web.Models.ViewModels.Payment
{
    public class PaymentDisplayViewModel
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
    }
} 