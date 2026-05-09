using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Evently.DB;
using Evently.Models;

namespace Evently.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _db;

        public ProfileController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            // Read the logged-in user's ID from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Account"); // redirect to login if no session

            // Load user + role from DB
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
                return NotFound();

            // Load this user's registrations + related events
            var registrations = await _db.Registrations
                .Include(r => r.Event)
                .Where(r => r.User.UserId == userId.Value)
                .ToListAsync();

            // Build event history list
            var eventHistory = registrations.Select(r =>
            {
                // Map EventStatus enum to display string
                string status = r.Event.Status switch
                {
                    Events.EventStatus.Scheduled => "Upcoming",
                    Events.EventStatus.Ongoing => "Ongoing",
                    Events.EventStatus.Completed => "Ended",
                    _ => r.Event.Status.ToString()
                };

                // Map RegistrationStatus to attendance display
                string attendance = r.Status switch
                {
                    Registrations.RegistrationStatus.Confirmed => "Yes",
                    Registrations.RegistrationStatus.Cancelled => "No",
                    _ => "Undetermined"
                };

                return new EventHistoryItem
                {
                    Name = r.Event.EventName,
                    Date = r.Event.EventDate.ToString("MMM dd"),
                    Status = status,
                    Attendance = attendance
                };
            }).ToList();

            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNum,
                Role = user.Role?.Role.ToString() ?? "User",
                EventCount = eventHistory.Count,
                EventHistory = eventHistory
            };

            return View(model);
        }
    }
}