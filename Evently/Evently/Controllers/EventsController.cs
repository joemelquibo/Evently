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
            evt.UserId = userId.Value; //
            evt.EventDate = DateTime.SpecifyKind(evt.EventDate, DateTimeKind.Utc);
            // Logic Validation
            if (evt.EndTime <= evt.StartTime)
            {
                ModelState.AddModelError("", "End time must be later than start time.");
            }

            // Clean up Validation
            // Remove the 'User' navigation object from validation to prevent null-object errors
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Events/CreateEvent.cshtml", evt);
            }

            try
            {
                _context.Events.Add(evt);
                _context.SaveChanges();

                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex) // <--- You need the 'ex' here!
            {
                // This extracts the real database error so we can see it on the screen
                var innerError = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Database Error: " + innerError);

                return View("~/Views/Home/Events/CreateEvent.cshtml", evt);
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

            if (evt == null)
            {
                return NotFound(); // This handles cases where the ID doesn't exist
            }

            return View("~/Views/Home/Events/Details.cshtml", evt);
        }
        [HttpPost]
        public IActionResult Join(int id)
        {
            // 1. Get the current User's ID from the session (saved as Int32 in AccountController)
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Account");
            }

            // 2. Fetch all necessary data from the Database
            var currentUser = _context.Users.Find(currentUserId.Value);
            var eventObj = _context.Events.Find(id);

            // Fetch a default Admin to satisfy the 'VerifiedBy' required member in your model
            var adminUser = _context.Users.FirstOrDefault(u => u.Role.Role == Evently.Models.Roles.RoleName.Admin);

            // 3. Safety checks
            if (eventObj == null) return NotFound();
            if (currentUser == null || adminUser == null)
            {
                TempData["Error"] = "Account verification failed. Please try logging in again.";
                return RedirectToAction("Index", "Account");
            }

            // 4. Registration Capacity Logic (The "1/5" Check)
            // Count current attendees to compare against the static Capacity limit
            var currentRegistrations = _context.Attendances.Count(a => a.EventId == id);

            if (currentRegistrations >= eventObj.Capacity)
            {
                TempData["Error"] = $"Sorry, {eventObj.EventName} is already fully booked!";
                return RedirectToAction("Index");
            }

            // 5. Initialize the Attendance record based on your Attendances.cs model
            var attendance = new Attendances
            {
                EventId = id,           // Uses the 'id' passed from the button
                User = currentUser,     // Satisfies 'required Users User'
                VerifiedBy = adminUser, // Satisfies 'required Users VerifiedBy'
                checkInTime = DateTime.UtcNow,
                Status = Attendances.AttendanceStatus.Present // Sets initial status
            };

            try
            {
                // 6. Save to Database
                _context.Attendances.Add(attendance);
                _context.SaveChanges();

                // 7. Set success message and Redirect to the main Events page
                TempData["Success"] = $"You've successfully joined {eventObj.EventName}! See you there.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the inner exception if the database rejects the save
                TempData["Error"] = "Database Error: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction("Details", new { id = id });
            }
        }
    }
}