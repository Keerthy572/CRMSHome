using Microsoft.AspNetCore.Mvc;

namespace CRMSHome.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CustomerDashboard()
        {
            return View();
        }
    }
}
