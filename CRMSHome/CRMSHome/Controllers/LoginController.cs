using Microsoft.AspNetCore.Mvc;
using CRMSHome.Models;

namespace CRMSHome.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Admin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserName == "admin" && model.Password == "123")
            {
                // Redirect to dashboard (HomeController → Dashboard)
                return RedirectToAction( "Index" , "Home");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View(model);
            }
        }
    }
}
