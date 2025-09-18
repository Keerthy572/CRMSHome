using System.Diagnostics;
using CRMSHome.Data;
using CRMSHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMSHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Home page
        public IActionResult Index()
        {
            return View();
        }

        // List all cars
        public IActionResult Privacy()
        {
            var cars = _context.Cars.ToList();
            return View(cars);
        }

        // View car details
        public IActionResult ViewCar(Guid id)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null) return NotFound();
            return View(car);
        }

        // ----------------- ADD CAR -----------------

        // GET: AddCar
        public IActionResult AddCar()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: AddCar
        [HttpPost]
        public IActionResult AddCar(Car car, IFormFile carImage)
        {
            if (ModelState.IsValid)
            {
                car.Id = Guid.NewGuid();

                // Handle image upload (optional)
                if (carImage != null && carImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(carImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cars", fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    carImage.CopyTo(stream);

                    car.ImagePath = "/images/cars/" + fileName;
                    car.BookingStatus = "Available";
                }

                _context.Cars.Add(car);
                _context.SaveChanges();
                return RedirectToAction("Privacy");
            }

            PopulateDropdowns(); // Important for re-displaying dropdowns
            return View(car);
        }

        // ----------------- EDIT CAR -----------------

        // GET: EditCar
        public IActionResult EditCar(Guid id)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null) return NotFound();

            PopulateDropdowns();
            return View(car);
        }

        // POST: EditCar
        [HttpPost]
        public IActionResult EditCar(Car updatedCar, IFormFile carImage)
        {
            if (ModelState.IsValid)
            {
                var car = _context.Cars.FirstOrDefault(c => c.Id == updatedCar.Id);
                if (car != null)
                {
                    car.Brand = updatedCar.Brand;
                    car.Model = updatedCar.Model;
                    car.CarType = updatedCar.CarType;
                    car.SeatCapacity = updatedCar.SeatCapacity;
                    car.RentPerDay = updatedCar.RentPerDay;
                    car.AvailableStatus = updatedCar.AvailableStatus;

                    // Update image if a new file is uploaded
                    if (carImage != null && carImage.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(carImage.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cars", fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        carImage.CopyTo(stream);

                        car.ImagePath = "/images/cars/" + fileName;
                    }

                    _context.Cars.Update(car);
                    _context.SaveChanges();
                }

                return RedirectToAction("Privacy");
            }

            PopulateDropdowns(); // Important if ModelState is invalid
            return View(updatedCar);
        }

        // ----------------- DELETE CAR -----------------
        public IActionResult DeleteCar(Guid id)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                _context.SaveChanges();
            }

            return RedirectToAction("Privacy");
        }

        // ----------------- HELPER METHODS -----------------
        private void PopulateDropdowns()
        {
            ViewBag.Brands = _context.Filters.Select(f => f.Brand).Distinct().ToList();
            ViewBag.SeatCapacities = _context.Filters
                                             .Where(f => f.SeatCapacity.HasValue)
                                             .Select(f => f.SeatCapacity.Value)
                                             .Distinct()
                                             .ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
