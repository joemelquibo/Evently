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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Account");

            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
                return NotFound();

            var registrations = await _db.Registrations
                .Include(r => r.Event)
                .Where(r => r.User.UserId == userId.Value)
                .ToListAsync();

            var eventHistory = registrations.Select(r =>
            {
                string status = r.Event.Status switch
                {
                    Events.EventStatus.Scheduled => "Upcoming",
                    Events.EventStatus.Ongoing => "Ongoing",
                    Events.EventStatus.Completed => "Ended",
                    _ => r.Event.Status.ToString()
                };

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
                EventHistory = eventHistory,
                Balance = user.Balance,
                ImageUrl = user.ImageUrl
            };

            return View(model);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Index", "Account");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNum,
                CurrentImageUrl = user.ImageUrl
            };

            return View(model);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Extra safety: make sure the session user matches the form's UserId
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null || sessionUserId.Value != model.UserId)
                return RedirectToAction("Index", "Account");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (user == null)
                return NotFound();

            // Profile Picture uploading logic
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                // Define folder path in wwwroot
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                // Create a unique filename to avoid overwriting
                string fileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(folder, fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                // Update the database property with the relative path
                user.ImageUrl = "/images/profiles/" + fileName;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNum = model.Phone;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        //CASH IN LOGIC
        // GET: /Profile/CashIn
        public async Task<IActionResult> CashIn()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index", "Account");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
            if (user == null) return NotFound();

            var model = new CashInViewModel
            {
                UserId = userId.Value,
                CurrentBalance = user.Balance    // ← reads from DB, not session
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CashIn(CashInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null || sessionUserId.Value != model.UserId)
                return RedirectToAction("Index", "Account");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (user == null)
                return NotFound();

            user.Balance += model.Amount;

            await _db.SaveChangesAsync();
            HttpContext.Session.SetString("UserBalance", user.Balance.ToString("N2"));
            TempData["Success"] = $"Succesfully cashed in Php{model.Amount:C2}!";
            return RedirectToAction("Index");
        }
    }
}