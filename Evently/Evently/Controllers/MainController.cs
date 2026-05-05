using Evently.DB;
using Evently.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Evently.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _context;

        public MainController(AppDbContext context)
        {
            _context = context;
        }

        
        // DASHBOARD
        
        public IActionResult Index()
        {
            // Optional: pass data later (stats, etc.)
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult Dashboard()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        
        // CALENDAR
        
        public IActionResult Calendar(int? month, int? year)
        {
            ViewBag.Month = month ?? DateTime.Today.Month;
            ViewBag.Year = year ?? DateTime.Today.Year;

            return View("~/Views/Home/Calendar.cshtml");
        }

        
        // PRIVACY
       
        public IActionResult Privacy()
        {
            return View("~/Views/Home/Privacy.cshtml");
        }

        
        // ERROR
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml",
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
        }
    }
}