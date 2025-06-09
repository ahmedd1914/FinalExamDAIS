using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinalExamDAIS.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
        }

        protected int? GetUserId()
        {
            // First try to get from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                return userId;
            }

            // If not in session, try to get from cookie
            var userIdCookie = Request.Cookies["UserId"];
            if (!string.IsNullOrEmpty(userIdCookie) && int.TryParse(userIdCookie, out int cookieUserId))
            {
                // Restore session from cookie
                HttpContext.Session.SetInt32("UserId", cookieUserId);
                HttpContext.Session.SetString("UserName", Request.Cookies["UserName"] ?? string.Empty);
                return cookieUserId;
            }

            return null;
        }

        protected IActionResult RequireUserId()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        protected IActionResult RequireUserId(int requestedUserId)
        {
            var userId = GetUserId();
            if (!userId.HasValue || userId.Value != requestedUserId)
            {
                return RedirectToAction("Login", "Account");
            }
            return null;
        }
    }
} 