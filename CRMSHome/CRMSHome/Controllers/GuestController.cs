using Microsoft.AspNetCore.Mvc;

namespace CRMSHome.Controllers
{
    public class GuestController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult GuestCars()
        {

            return View();
        }

    }
}
