using FinalExamDAIS.Services.DTOs.Account;
using System.Collections.Generic;

namespace FinalExamDAIS.Web.Models.ViewModels.BankAccount
{
    public class AccountListViewModel
    {
        public List<AccountInfo> Accounts { get; set; } = new List<AccountInfo>();
        public string ErrorMessage { get; set; }
    }
} 