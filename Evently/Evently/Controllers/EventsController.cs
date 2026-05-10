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
                .Include(e => e.User) // useful later (creator name)
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
                return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
    }
}