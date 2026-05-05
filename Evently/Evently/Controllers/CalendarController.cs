using Microsoft.AspNetCore.Mvc;

namespace Evently.Controllers
{
    public class Calendar : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
