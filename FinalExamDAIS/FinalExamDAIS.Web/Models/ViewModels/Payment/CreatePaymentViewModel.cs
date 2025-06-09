using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FinalExamDAIS.Web.Models.ViewModels.Account;

namespace FinalExamDAIS.Web.Models.ViewModels.Payment
{
    public class CreatePaymentViewModel
    {
        [Display(Name = "Изберете акаунт")]
        [Required(ErrorMessage = "Моля, изберете акаунт")]
        public int FromAccountId { get; set; }

        [Required(ErrorMessage = "Моля, въведете номер на акаунт")]
        [StringLength(22, MinimumLength = 22, ErrorMessage = "Номерът на акаунта трябва да бъде точно 22 символа")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Номерът на акаунта трябва да съдържа само букви и цифри")]
        public string ToAccountNumber { get; set; }

        [Required(ErrorMessage = "Моля, въведете сума")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумата трябва да бъде по-голяма от 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Моля, въведете причина")]
        [StringLength(32, ErrorMessage = "Причината не може да бъде по-дълга от 32 символа")]
        public string Reason { get; set; }

    }
} 