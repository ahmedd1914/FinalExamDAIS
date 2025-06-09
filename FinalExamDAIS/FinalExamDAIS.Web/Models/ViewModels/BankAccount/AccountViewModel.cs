using System.ComponentModel.DataAnnotations;

namespace FinalExamDAIS.Web.Models.ViewModels.Account
{
    public class AccountViewModel
    {
        public int AccountId { get; set; }
        
        [Display(Name = "Номер на акаунт")]
        public string AccountNumber { get; set; }
        
        [Display(Name = "Налична сума")]
        public decimal AvailableAmount { get; set; }
        
        [Display(Name = "Активен")]
        public bool IsActive { get; set; }

        public string AccountNumberWithBalance => $"{AccountNumber} - {AvailableAmount.ToString("N2")} лв.";
    }
} 