using Microsoft.AspNetCore.Mvc;
using Evently.DB;
using Evently.Models;

namespace Evently.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //Sign in Logic
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
                ViewBag.Error = "Your account has been Suspended. Please contact support";
                return View();
            }

            if (user.Password != password)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FirstName);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        //Register Logic
        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password, /*string confpassword,*/ string phoneNum, Roles.RoleName role, Users.UserStatus status = Users.UserStatus.Active)
        {
            var roleEntity = _context.Roles.FirstOrDefault(r => r.Role == role);

            var newUser = new Users 
            { 
                FirstName= firstName,
                LastName= lastName,
                Email= email,
                Password= password,
                //ConfirmPassword= confpassword,
                PhoneNum= phoneNum,
                Role = roleEntity,
                Status = status
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return View("Index");
        }

    }
}
