using Microsoft.AspNetCore.Mvc;
using FinalExamDAIS.Services.Interfaces.Authentication;
using FinalExamDAIS.Services.DTOs.Authentication;
using FinalExamDAIS.Web.Models.ViewModels.Account;
using AuthService = FinalExamDAIS.Services.Interfaces.Authentication.IAuthenticationService;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace FinalExamDAIS.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            try
            {
                if (GetUserId().HasValue)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(new LoginViewModel
                {
                    ReturnUrl = returnUrl
                });
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Username))
                {
                    ModelState.AddModelError("Username", "Потребителското име е задължително");
                    return View(model);
                }

                if (model.Username.Length < 3 || model.Username.Length > 50)
                {
                    ModelState.AddModelError("Username", "Потребителското име трябва да бъде между 3 и 50 символа");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Паролата е задължителна");
                    return View(model);
                }

                if (model.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Паролата трябва да бъде поне 6 символа");
                    return View(model);
                }

                var result = await _authService.LoginAsync(new LoginRequest
                {
                    Username = model.Username,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                });

                if (result.Success)
                {
                    HttpContext.Session.SetInt32("UserId", result.UserInfo.UserId);
                    HttpContext.Session.SetString("UserName", result.UserInfo.Username);

                    if (model.RememberMe)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(30),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        };

                        Response.Cookies.Append("UserId", result.UserInfo.UserId.ToString(), cookieOptions);
                        Response.Cookies.Append("UserName", result.UserInfo.Username, cookieOptions);
                    }

                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", result.Message ?? "Невалидно потребителско име или парола");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Възникна грешка при влизане. Моля, опитайте отново.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                if (GetUserId().HasValue)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Username))
                {
                    ModelState.AddModelError("Username", "Потребителското име е задължително");
                    return View(model);
                }

                if (model.Username.Length < 3 || model.Username.Length > 50)
                {
                    ModelState.AddModelError("Username", "Потребителското име трябва да бъде между 3 и 50 символа");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError("Email", "Имейлът е задължителен");
                    return View(model);
                }

                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(model.Email))
                {
                    ModelState.AddModelError("Email", "Невалиден формат на имейл");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Паролата е задължителна");
                    return View(model);
                }

                if (model.Password.Length < 6)
                {
                    ModelState.AddModelError("Password", "Паролата трябва да бъде поне 6 символа");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.FirstName))
                {
                    ModelState.AddModelError("FirstName", "Името е задължително");
                    return View(model);
                }

                if (model.FirstName.Length < 2 || model.FirstName.Length > 50)
                {
                    ModelState.AddModelError("FirstName", "Името трябва да бъде между 2 и 50 символа");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.LastName))
                {
                    ModelState.AddModelError("LastName", "Фамилията е задължителна");
                    return View(model);
                }

                if (model.LastName.Length < 2 || model.LastName.Length > 50)
                {
                    ModelState.AddModelError("LastName", "Фамилията трябва да бъде между 2 и 50 символа");
                    return View(model);
                }

                if (model.DateOfBirth == default)
                {
                    ModelState.AddModelError("DateOfBirth", "Датата на раждане е задължителна");
                    return View(model);
                }

                var minAge = 18;
                var maxAge = 100;
                var age = DateTime.Today.Year - model.DateOfBirth.Year;
                if (model.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

                if (age < minAge || age > maxAge)
                {
                    ModelState.AddModelError("DateOfBirth", $"Възрастта трябва да бъде между {minAge} и {maxAge} години");
                    return View(model);
                }

                var request = new RegisterRequest
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth
                };

                var result = await _authService.RegisterAsync(request);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Регистрацията беше успешна! Моля, влезте в системата.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", result.Message ?? "Регистрацията не беше успешна");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Възникна грешка при регистрация. Моля, опитайте отново.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("UserId");
                Response.Cookies.Delete("UserName");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login");
            }
        }
    }
}
