using Microsoft.AspNetCore.Mvc;
using Evently.DB;
using Evently.Models;
using Microsoft.EntityFrameworkCore;

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
                      .Include(e => e.Attendances!)
                      .ThenInclude(a => a.User)
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
                _context.Attendances.Count();

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

            // RECENT EVENTS
            ViewBag.RecentEvents =
                _context.Events
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

            // EVENT PARTICIPANTS
            ViewBag.EventParticipants =
                _context.Attendances
                    .GroupBy(a => a.EventId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );

            // EVENT PRESENT
            ViewBag.EventPresent =
                _context.Attendances
                    .Where(a =>
                        a.Status ==
                        Attendances.AttendanceStatus.Present)
                    .GroupBy(a => a.EventId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );

            // EVENT ABSENT
            ViewBag.EventAbsent =
                _context.Attendances
                    .Where(a =>
                        a.Status ==
                        Attendances.AttendanceStatus.Absent)
                    .GroupBy(a => a.EventId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Count()
                    );

            // SEND EVENTS TO VIEW
            return View(
                "~/Views/Home/Index.cshtml",
                events
            );
        }
    }
}