using Microsoft.AspNetCore.Mvc;
using CRMSHome.Data;
using CRMSHome.Models;

namespace CRMSHome.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Login (GET)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Login (POST)
        [HttpPost]
        public IActionResult Index(Admin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Admin Login
            if (model.UserName == "admin" && model.Password == "123")
            {
                return RedirectToAction("Index", "Home"); // Admin dashboard
            }

            // Customer Login
            var customer = _context.Customers
                .FirstOrDefault(c => c.Username == model.UserName && c.Password == model.Password);

            if (customer != null)
            {
                return RedirectToAction("CustomerDashboard", "Customer"); // ✅ You can create this page
            }

            ViewBag.Error = "Invalid username or password";
            return View(model);
        }

        // Register (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register (POST)
        [HttpPost]
        public IActionResult Register(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prevent duplicate usernames (including admin)
            if (model.Username.ToLower() == "admin" ||
                _context.Customers.Any(c => c.Username == model.Username))
            {
                ViewBag.Error = "Username already exists. Please choose another.";
                return View(model);
            }

            model.Id = Guid.NewGuid();
            _context.Customers.Add(model);
            _context.SaveChanges();

            // Redirect to login after registration
            return RedirectToAction("Index", "Login");
        }
    }
}
