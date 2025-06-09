using System.Diagnostics;
using FinalExamDAIS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using FinalExamDAIS.Services.Interfaces.Account;
using FinalExamDAIS.Web.Models.ViewModels.BankAccount;
using FinalExamDAIS.Web.Attributes;

namespace FinalExamDAIS.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IAccountService _accountService;

        public HomeController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var userId = GetUserId().Value;
                var viewModel = new AccountListViewModel();
                var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                viewModel.Accounts = accounts.Accounts;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> AccountDetails(int id)
        {
            try
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var viewModel = new AccountDetailsViewModel();
                var account = await _accountService.GetAccountByIdAsync(id);
                
                if (account == null)
                {
                    viewModel.ErrorMessage = "Account not found";
                    return View(viewModel);
                }

                viewModel.Account = account;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
