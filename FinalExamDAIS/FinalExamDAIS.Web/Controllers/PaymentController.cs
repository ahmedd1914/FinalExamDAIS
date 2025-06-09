using FinalExamDAIS.Services.Interfaces.Payment;
using FinalExamDAIS.Services.Interfaces.Account;
using Microsoft.AspNetCore.Mvc;
using FinalExamDAIS.Web.Models;
using FinalExamDAIS.Web.Models.ViewModels.Payment;
using FinalExamDAIS.Web.Models.ViewModels.Account;
using FinalExamDAIS.Services.DTOs.Payment;
using System.Diagnostics;
using FinalExamDAIS.Web.Attributes;
using FinalExamDAIS.Models.Enums;
using FinalExamDAIS.Repository.Interfaces.User;

namespace FinalExamDAIS.Web.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IAccountService _accountService;
        private readonly IUserRepository _userRepository;

        public PaymentController(
            IPaymentService paymentService, 
            IAccountService accountService,
            IUserRepository userRepository)
        {
            _paymentService = paymentService;
            _accountService = accountService;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(string sortBy = "date")
        {
            try 
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var userId = GetUserId().Value;
                var viewModel = new PaymentListViewModel();
                var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);

                // Get account numbers for display
                var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                var accountNumbers = accounts.Accounts.ToDictionary(a => a.AccountId, a => a.AccountNumber);

                var paymentViewModels = payments.Select(p => new PaymentDisplayViewModel
                {
                    PaymentId = p.PaymentId,
                    FromAccountId = p.FromAccountId,
                    FromAccountNumber = accountNumbers.GetValueOrDefault(p.FromAccountId, "Unknown"),
                    ToAccountNumber = p.ToAccountNumber,
                    Amount = p.Amount,
                    Reason = p.Reason,
                    Status = p.Status,
                    CreatedDate = p.CreatedDate,
                    ProcessedDate = p.ProcessedDate,
                    CreatedByUserId = p.CreatedByUserId
                });

                if (sortBy == "status")
                    viewModel.Payments = paymentViewModels
                        .OrderBy(p => p.Status == "Pending" ? 0 : p.Status == "Processed" ? 1 : 2)
                        .ThenByDescending(p => p.CreatedDate)
                        .ToList();
                else
                    viewModel.Payments = paymentViewModels.OrderByDescending(p => p.CreatedDate).ToList();

                viewModel.SortBy = sortBy;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Възникна грешка при зареждане на плащанията. Моля, опитайте отново."
                });
            }
        }

        public async Task<IActionResult> CreatePayment()
        {
            try 
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var userId = GetUserId().Value;
                var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                ViewBag.Accounts = accounts.Accounts.Select(a => new AccountViewModel
                {
                    AccountId = a.AccountId,
                    AccountNumber = $"{a.AccountNumber} - {a.AvailableAmount.ToString("N2")} лв.",
                    AvailableAmount = a.AvailableAmount,
                    IsActive = a.IsActive
                }).ToList();
                
                return View(new CreatePaymentViewModel());
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Възникна грешка при зареждане на формата. Моля, опитайте отново."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(CreatePaymentViewModel model)
        {
            try 
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                if (!ModelState.IsValid)
                {
                    var user = GetUserId().Value;
                    var accounts = await _accountService.GetAccountsByUserIdAsync(user);
                    ViewBag.Accounts = accounts.Accounts.Select(a => new AccountViewModel
                    {
                        AccountId = a.AccountId,
                        AccountNumber = $"{a.AccountNumber} - {a.AvailableAmount.ToString("N2")} лв.",
                        AvailableAmount = a.AvailableAmount,
                        IsActive = a.IsActive
                    }).ToList();
                    return View(model);
                }

                var userId = GetUserId().Value;
                var payment = await _paymentService.CreatePaymentAsync(new CreatePaymentDto
                {
                    FromAccountId = model.FromAccountId,
                    ToAccountNumber = model.ToAccountNumber,
                    Amount = model.Amount,
                    Reason = model.Reason,
                    CreatedByUserId = userId
                });

                if (!payment.Success)
                {
                    ModelState.AddModelError("", payment.Message);
                    var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                    ViewBag.Accounts = accounts.Accounts.Select(a => new AccountViewModel
                    {
                        AccountId = a.AccountId,
                        AccountNumber = $"{a.AccountNumber} - {a.AvailableAmount.ToString("N2")} лв.",
                        AvailableAmount = a.AvailableAmount,
                        IsActive = a.IsActive
                    }).ToList();
                    return View(model);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Възникна грешка при създаване на плащането. Моля, опитайте отново."
                });
            }
        }

        public async Task<IActionResult> ProcessPayment(int paymentId)
        {
            try 
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var payment = await _paymentService.ProcessPaymentAsync(paymentId);
                if (!payment.Success)
                {
                    TempData["Error"] = payment.Message;
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Възникна грешка при обработка на плащането. Моля, опитайте отново."
                });
            }
        }

        public async Task<IActionResult> RejectPayment(int paymentId)
        {
            try 
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var payment = await _paymentService.RejectPaymentAsync(paymentId);
                if (!payment.Success)
                {
                    TempData["Error"] = payment.Message;
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Възникна грешка при отхвърляне на плащането. Моля, опитайте отново."
                });
            }
        }

        public async Task<IActionResult> Details(int paymentId)
        {
            try 
            {
                var requiredUserId = RequireUserId();
                if (requiredUserId != null) return requiredUserId;

                var userId = GetUserId().Value;
                var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
                
                if (payment == null)
                {
                    return View("Error", new ErrorViewModel 
                    { 
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        Message = "Плащането не е намерено."
                    });
                }

                // Get account numbers for display
                var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
                var accountNumbers = accounts.Accounts.ToDictionary(a => a.AccountId, a => a.AccountNumber);

                // Get user information
                var user = await _userRepository.RetrieveAsync(payment.CreatedByUserId);
                var username = user?.Username ?? "Unknown User";

                var viewModel = new PaymentDetailsViewModel
                {
                    PaymentId = payment.PaymentId,
                    FromAccountNumber = accountNumbers.GetValueOrDefault(payment.FromAccountId, "Unknown"),
                    ToAccountNumber = payment.ToAccountNumber,
                    Amount = payment.Amount,
                    Reason = payment.Reason,
                    Status = payment.Status,
                    CreatedDate = payment.CreatedDate,
                    ProcessedDate = payment.ProcessedDate,
                    CreatedByUser = username
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel 
                { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Възникна грешка при зареждане на детайлите на плащането. Моля, опитайте отново."
                });
            }
        }
    }
}   