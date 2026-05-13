using Microsoft.AspNetCore.Mvc;
using Evently.DB;
using Evently.Models;
using Microsoft.EntityFrameworkCore;

namespace Evently.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        
        // LIST ALL EVENTS
        
        public IActionResult Index()
        {
            var events = _context.Events
                .Include(e => e.User) 
                .ToList();

            var registrations = _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.Status == Registrations.RegistrationStatus.Confirmed)
                .ToList();

            ViewBag.Registrations = registrations;

            return View("~/Views/Home/Events/Index.cshtml", events);
        }

       
        // CREATE (GET)
        
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if(role != "Admin" && role != "Organizer")
            {
                return RedirectToAction("Index", "Events");
            }
            return View("~/Views/Home/Events/CreateEvent.cshtml");
        }


        // CREATE (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Events evt)
        {
            // Check Role Authorization
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Organizer")
            {
                TempData["Error"] = "Access Denied: Only Admins and Organizers can create events.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Assign the UserId BEFORE validation
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Account");
            }

            // BALANCE Check logic
            var user = _context.Users.Find(userId.Value);
            if (user == null) return NotFound();

            // Extract the Payment Amount from the form
            decimal.TryParse(Request.Form["PayAmt"], out decimal payAmt);

            // Validate if the user has enough balance
            if (user.Balance < payAmt)
            {
                ModelState.AddModelError("", $"Insufficient balance. Your current balance is ₱{user.Balance:N2}.");
                return View("~/Views/Home/Events/CreateEvent.cshtml", evt);
            }

            evt.UserId = userId.Value;
            evt.EventDate = DateTime.SpecifyKind(evt.EventDate, DateTimeKind.Utc);
            // Logic Validation
            if (evt.EndTime <= evt.StartTime)
            {
                ModelState.AddModelError("", "End time must be later than start time.");
            }

            // Remove the 'User' navigation object from validation to prevent null-object errors
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Events/CreateEvent.cshtml", evt);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Subtracts the amount from the User's balance
                    user.Balance -= payAmt;
                    _context.Users.Update(user);

                    _context.Events.Add(evt);
                    _context.SaveChanges();

                    // Commit transaction
                    transaction.Commit();

                    HttpContext.Session.SetString("UserBalance", user.Balance.ToString("N2"));

                    TempData["Success"] = "Event created and balance updated!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var innerError = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", "Database Error: " + innerError);
                    return View("~/Views/Home/Events/CreateEvent.cshtml", evt);
                }
            }
        }
        
       // ATTENDANCE DETAILS
public IActionResult AttendanceDetails(int id)
{
    // GET EVENT
    var evt = _context.Events
        .FirstOrDefault(e => e.EventId == id);

    if (evt == null)
    {
        return NotFound();
    }

    // GET ATTENDANCES
    var attendances = _context.Attendances
        .Include(a => a.User)
        .Where(a => a.EventId == id)
        .ToList();

    // TOTAL PARTICIPANTS
    var totalParticipants =
        attendances.Count;

    // PRESENT COUNT
    var presentCount =
        attendances.Count(a =>
            a.Status ==
            Attendances.AttendanceStatus.Present);

    // ABSENT COUNT
    var absentCount =
        attendances.Count(a =>
            a.Status ==
            Attendances.AttendanceStatus.Absent);

    // ATTENDANCE RATE
    double attendanceRate = 0;

    if (totalParticipants > 0)
    {
        attendanceRate =
            ((double)presentCount
            / totalParticipants) * 100;
    }

            // VIEW MODEL
            var vm =
                new Evently.Models.ViewModels
                .EventAttendanceViewModel
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
                "~/Views/Home/Events/AttendanceDetails.cshtml",
                vm
            );
        }

        // EDIT (GET)

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var evt = _context.Events.FirstOrDefault(e => e.EventId == id);

            if (evt == null)
                return NotFound();

            return View("~/Views/Home/Events/Edit.cshtml", evt);
        }


        // EDIT (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Events evt)
        {
            // 1. MUST ADD THIS: The Postgres UTC Passport
            evt.EventDate = DateTime.SpecifyKind(evt.EventDate, DateTimeKind.Utc);

            if (evt.EndTime <= evt.StartTime)
            {
                ModelState.AddModelError("", "End time must be later than start time.");
            }

            // 2. Clean up validation for navigation properties
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Events/Edit.cshtml", evt);
            }

            try
            {
                // 3. Ensure the record is being tracked for update
                _context.Events.Update(evt);
                _context.SaveChanges();

                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var innerError = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Update failed: " + innerError);
                return View("~/Views/Home/Events/Edit.cshtml", evt);
            }
        }


        // DELETE

        public IActionResult Delete(int id)
        {
            var evt = _context.Events.FirstOrDefault(e => e.EventId == id);

            if (evt != null)
            {
                _context.Events.Remove(evt);
                _context.SaveChanges();
                TempData["Success"] = "Event deleted.";
            }

            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult Details(int id)
        {
            // Find the event by ID
            var evt = _context.Events.FirstOrDefault(e => e.EventId == id);

            if (evt == null) return NotFound();
            // Count confirmed registrations for this event
            ViewBag.RegCount = _context.Registrations.Count(r => r.EventId == id);

            return View("~/Views/Home/Events/Details.cshtml", evt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userRole = HttpContext.Session.GetString("UserRole");

            // Check if event exists and if user is already registered
            var eventObj = _context.Events.Find(id);
            if (eventObj == null) return NotFound();

            var alreadyRegistered = _context.Registrations
                .Any(r => r.EventId == id && r.UserId == userId);

            if (alreadyRegistered)
            {
                TempData["Info"] = "You are already registered for this event.";
                return RedirectToAction("Details", new { id = id });
            }

            // Create the registration record
            var registration = new Registrations
            {
                EventId = id,
                UserId = userId!.Value,
                RegistrationDate = DateTime.UtcNow,
                Status = Registrations.RegistrationStatus.Confirmed,
                Event = eventObj,
                User = _context.Users.Find(userId.Value)!
            };

            try
            {
                _context.Registrations.Add(registration);
                _context.SaveChanges();
                TempData["Success"] = $"Successfully joined {eventObj.EventName}!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Registration failed: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}