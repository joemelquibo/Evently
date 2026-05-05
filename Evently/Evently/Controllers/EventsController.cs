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
            return View("~/Views/Home/Events/CreateEvent.cshtml");
        }

        
        // CREATE (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Events evt)
        {
            // ❗ VALIDATE TIME
            if (evt.EndTime <= evt.StartTime)
            {
                ModelState.AddModelError("", "End time must be later than start time.");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Events/CreateEvent.cshtml", evt);
            }

            try
            {
                // ✅ FIX: Assign logged-in user
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                {
                    return RedirectToAction("Index", "Account");
                }

                evt.UserId = userId.Value;

                _context.Events.Add(evt);
                _context.SaveChanges();

                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
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

            return View("~/Views/Home/Events/EditEvent.cshtml", evt);
        }

        
        // EDIT (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Events evt)
        {
            if (evt.EndTime <= evt.StartTime)
            {
                ModelState.AddModelError("", "End time must be later than start time.");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Events/EditEvent.cshtml", evt);
            }

            try
            {
                _context.Events.Update(evt);
                _context.SaveChanges();

                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Update failed.");
                return View("~/Views/Home/Events/EditEvent.cshtml", evt);
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