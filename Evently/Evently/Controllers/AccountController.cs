using Microsoft.AspNetCore.Mvc;
using Evently.DB;
using Evently.Models;
using System.Security.Cryptography;
using System.Text;

namespace Evently.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        private static string HashPassword(string password)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Sign in Logic
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            if (user.Status == Users.UserStatus.Inactive)
            {
                ViewBag.Error = "Your account is Inactive. Please contact support.";
                return View();
            }

            if (user.Status == Users.UserStatus.Suspended)
            {
                ViewBag.Error = "Your account has been Suspended. Please contact support.";
                return View();
            }

            if (user.Password != HashPassword(password))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FirstName);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register Logic
        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password, string confpassword, string phoneNum, Roles.RoleName role, Users.UserStatus status = Users.UserStatus.Active)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            if (password != confpassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "An account with this email already exists.";
                return View();
            }

            var roleEntity = _context.Roles.FirstOrDefault(r => r.role == role);

            if (roleEntity == null)
            {
                ViewBag.Error = "Selected role not found.";
                return View();
            }

            var newUser = new Users
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = HashPassword(password),
                PhoneNum = phoneNum,
                Role = roleEntity,
                Status = status
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}