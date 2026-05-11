using Evently.Models;
using Evently.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Evently.Models.ViewModels;

namespace Evently.Controllers
{
    public class HomeController : Controller
    {
        // DATABASE
        private readonly AppDbContext _context;

        // CONSTRUCTOR
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // DASHBOARD
        // =========================
        public IActionResult Index()
        {
            // GET EVENTS
            var events = _context.Events.ToList();

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

            // PRESENT
            var presentCount =
                _context.Attendances
                    .Count(a =>
                        a.Status ==
                        Attendances.AttendanceStatus.Present);

            // ABSENT
            var absentCount =
                _context.Attendances
                    .Count(a =>
                        a.Status ==
                        Attendances.AttendanceStatus.Absent);

            // TOTAL PARTICIPANTS
            var totalParticipants =
                _context.Registrations.Count();

            // ATTENDANCE RATE
            double attendanceRate = 0;

            if (totalParticipants > 0)
            {
                attendanceRate =
                    (double)presentCount
                    / totalParticipants * 100;
            }

            ViewBag.AttendanceRate =
                Math.Round(attendanceRate, 0);

            ViewBag.PresentCount =
                presentCount;

            ViewBag.AbsentCount =
                absentCount;

            // SEND EVENTS TO VIEW
            return View(events);
        }

        // =========================
        // ATTENDANCE DETAILS
        // =========================
        public IActionResult AttendanceDetails(int id)
        {
            // EVENT
            var evt =
                _context.Events
                    .FirstOrDefault(e =>
                        e.EventId == id);

            if (evt == null)
            {
                return NotFound();
            }

            // ATTENDANCES
            var attendances =
                _context.Attendances
                    .Include(a => a.User)
                    .Where(a =>
                        a.EventId == id)
                    .ToList();

            // TOTAL
            var totalParticipants =
                attendances.Count;

            // PRESENT
            var presentCount =
                attendances.Count(a =>
                    a.Status ==
                    Attendances.AttendanceStatus.Present);

            // ABSENT
            var absentCount =
                attendances.Count(a =>
                    a.Status ==
                    Attendances.AttendanceStatus.Absent);

            // RATE
            double attendanceRate = 0;

            if (totalParticipants > 0)
            {
                attendanceRate =
                    (double)presentCount
                    / totalParticipants * 100;
            }

            // VIEW MODEL
            var model =
                new EventAttendanceViewModel
                {
                    Event = evt,

                    Attendances = attendances,

                    TotalParticipants =
                        totalParticipants,

                    PresentCount =
                        presentCount,

                    AbsentCount =
                        absentCount,

                    AttendanceRate =
                        attendanceRate
                };

            return View(
                "~/Views/Events/AttendanceDetails.cshtml",
                model
            );
        }

        // =========================
        // PRIVACY
        // =========================
        public IActionResult Privacy()
        {
            return View();
        }

        // =========================
        // ERROR
        // =========================
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                });
        }

        // =========================
        // CALENDAR
        // =========================
        public IActionResult Calendar(
            int? month,
            int? year)
        {
            var currentMonth = month ?? DateTime.Today.Month;

            var currentYear = year ?? DateTime.Today.Year;

            var eventsForMonth =
                _context.Events
                    .Where(e =>
                        e.EventDate.Month == currentMonth
                        &&
                        e.EventDate.Year == currentYear)
                    .ToList();

            ViewBag.Month =
                currentMonth;

            ViewBag.Year =
                currentYear;

            return View(eventsForMonth);
        }
    }
}