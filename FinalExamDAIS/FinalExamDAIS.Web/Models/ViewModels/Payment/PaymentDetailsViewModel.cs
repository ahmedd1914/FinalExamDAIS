using System.ComponentModel.DataAnnotations;

namespace FinalExamDAIS.Web.Models.ViewModels.Payment
{
    public class PaymentDetailsViewModel
    {
        public int PaymentId { get; set; }
        
        [Display(Name = "От акаунт")]
        public string FromAccountNumber { get; set; }
        
        [Display(Name = "Към акаунт")]
        public string ToAccountNumber { get; set; }
        
        [Display(Name = "Сума")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Причина")]
        public string Reason { get; set; }
        
        [Display(Name = "Статус")]
        public string Status { get; set; }
        
        [Display(Name = "Дата на създаване")]
        public DateTime CreatedDate { get; set; }
        
        [Display(Name = "Дата на обработка")]
        public DateTime? ProcessedDate { get; set; }
        
        [Display(Name = "Създаден от")]
        public string CreatedByUser { get; set; }
        
        [Display(Name = "Съобщение за грешка")]
        public string ErrorMessage { get; set; }
    }
} 