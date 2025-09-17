using Microsoft.AspNetCore.Mvc;

namespace CRMSHome.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
