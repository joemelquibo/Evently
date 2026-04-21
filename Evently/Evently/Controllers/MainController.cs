using Evently.DB;
using Microsoft.AspNetCore.Mvc;
using Evently.Models;

namespace Evently.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDbContext _context;
        public MainController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
