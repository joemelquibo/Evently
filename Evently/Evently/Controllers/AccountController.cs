using Microsoft.AspNetCore.Mvc;

namespace Evently.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(); // This looks for Views/Account/Register.cshtml
        }

    }
}
