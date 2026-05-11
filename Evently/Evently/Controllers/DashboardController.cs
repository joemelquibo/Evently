using Microsoft.AspNetCore.Mvc;
using Evently.DB;
using Evently.Models;

namespace Evently.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Account");

            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin" && role != "Organizer")
            {
                // Redirect non-staff users to the events page
                return RedirectToAction("Index", "Events");
            }

            // Stats using ViewBag — from notes pattern
            ViewBag.TotalEvents = _context.Events.Count();

            ViewBag.ActiveEvents = _context.Events
                .Count(e => e.Status == Events.EventStatus.Ongoing);

            ViewBag.Participants = _context.Registrations.Count();

            ViewBag.AttendanceRate = _context.Attendances.Any()
                ? (int)Math.Round(
                    _context.Attendances
                        .Count(a => a.Status == Attendances.AttendanceStatus.Present)
                    * 100.0 / _context.Attendances.Count())
                : 0;

            // This avoids casting issues with nullable UserId
            ViewBag.RecentEvents = _context.Events
                .OrderByDescending(e => e.EventDate)
                .Take(5)
                .Select(e => new
                {
                    e.EventId,
                    e.EventName,
                    e.EventDate,
                    e.StartTime,
                    e.Venue,
                    e.Status
                })
                .ToList();

            ViewBag.PresentCount = _context.Attendances
                .Count(a => a.Status == Attendances.AttendanceStatus.Present);

            ViewBag.AbsentCount = _context.Attendances
                .Count(a => a.Status == Attendances.AttendanceStatus.Absent);

            return View("~/Views/Home/Index.cshtml");
        }
    }
}