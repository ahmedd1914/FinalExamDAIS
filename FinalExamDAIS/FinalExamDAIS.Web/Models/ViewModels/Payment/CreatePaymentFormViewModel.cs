using System.ComponentModel.DataAnnotations;
using FinalExamDAIS.Web.Models.ViewModels.Account;

namespace FinalExamDAIS.Web.Models.ViewModels.Payment
{
    public class CreatePaymentFormViewModel
    {
        public CreatePaymentViewModel Payment { get; set; } = new();
        public List<AccountViewModel> Accounts { get; set; } = new();
    }
} 