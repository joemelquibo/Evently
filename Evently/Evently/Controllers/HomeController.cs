using Evently.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Evently.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Calendar(int? month, int? year)
        {
            // Pass the month and year to the View so the Razor code can use them
            ViewBag.Month = month ?? DateTime.Today.Month;
            ViewBag.Year = year ?? DateTime.Today.Year;

            return View();
        }
    }
}
