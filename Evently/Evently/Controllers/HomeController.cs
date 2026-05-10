using Evently.Models;
using Evently.DB; // Make sure this namespace matches your AppDbContext location
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Evently.Controllers
{
    public class HomeController : Controller
    {
        // 1. Declare the database context field
        private readonly AppDbContext _context;

        // 2. Add the Constructor to "Inject" the database
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

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
            var currentMonth = month ?? DateTime.Today.Month;
            var currentYear = year ?? DateTime.Today.Year;

            // Now _context will work because it was initialized in the constructor!
            var eventsForMonth = _context.Events
                .Where(e => e.EventDate.Month == currentMonth && e.EventDate.Year == currentYear)
                .ToList();

            ViewBag.Month = currentMonth;
            ViewBag.Year = currentYear;

            return View(eventsForMonth);
        }
    }
}