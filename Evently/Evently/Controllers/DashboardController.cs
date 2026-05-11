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

        // DASHBOARD
        [HttpGet]
        public IActionResult Index()
        {
            // CHECK LOGIN
            var userId =
                HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction(
                    "Index",
                    "Account"
                );
            }

            // CHECK ROLE
            var role =
                HttpContext.Session.GetString("UserRole");

            if (role != "Admin" &&
                role != "Organizer")
            {
                // PARTICIPANTS REDIRECT
                return RedirectToAction(
                    "Index",
                    "Events"
                );
            }

            // GET EVENTS
            var events =
                _context.Events
                    .OrderByDescending(e => e.EventDate)
                    .ToList();

            // STATS

            // TOTAL EVENTS
            ViewBag.TotalEvents =
                events.Count;

            // ACTIVE EVENTS
            ViewBag.ActiveEvents =
                events.Count(e =>
                    e.Status ==
                    Events.EventStatus.Ongoing
                    ||
                    e.Status ==
                    Events.EventStatus.Scheduled);

            // PARTICIPANTS
            ViewBag.Participants =
                _context.Registrations.Count();

            // PRESENT COUNT
            ViewBag.PresentCount =
                _context.Attendances
                    .Count(a =>
                        a.Status ==
                        Attendances.AttendanceStatus.Present);

            // ABSENT COUNT
            ViewBag.AbsentCount =
                _context.Attendances
                    .Count(a =>
                        a.Status ==
                        Attendances.AttendanceStatus.Absent);

            // ATTENDANCE RATE
            var totalAttendance =
                _context.Attendances.Count();

            ViewBag.AttendanceRate =
                totalAttendance > 0
                ? (int)Math.Round(
                    ViewBag.PresentCount
                    * 100.0
                    / totalAttendance)
                : 0;

            // SEND EVENTS TO VIEW
            return View(
                "~/Views/Home/Index.cshtml",
                events
            );
        }
    }
}