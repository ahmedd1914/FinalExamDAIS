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
            return HttpContext.Session.GetInt32("UserId");
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