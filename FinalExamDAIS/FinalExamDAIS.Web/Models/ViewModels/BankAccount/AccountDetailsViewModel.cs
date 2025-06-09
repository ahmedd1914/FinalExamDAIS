using FinalExamDAIS.Services.DTOs.Account;

namespace FinalExamDAIS.Web.Models.ViewModels.BankAccount
{
    public class AccountDetailsViewModel
    {
        public AccountInfo Account { get; set; }
        public string ErrorMessage { get; set; }
    }
} 