using CRMSHome.Data;
using CRMSHome.Models;
using CRMSHome.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            var model = new DashboardViewModel
            {
                TotalCars = _context.Cars.Count(),
                AvailableCars = _context.Cars.Count(c => c.AvailableStatus == "Available"),
                NotAvailableCars = _context.Cars.Count(c => c.AvailableStatus == "Not Available"),
                BookedCars = _context.Cars.Count(c => c.BookingStatus == "Booked"),
                NotBookedCars = _context.Cars.Count(c => c.BookingStatus == "Available")
            };

            return View(model);
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

        public IActionResult AllBookings()
        {
            var bookings = _context.Bookings
                .Join(_context.Customers,
                      b => b.CustomerId,
                      c => c.Id,
                      (b, c) => new { Booking = b, Customer = c })
                .Join(_context.Cars,
                      bc => bc.Booking.CarId,
                      car => car.Id,
                      (bc, car) => new { bc.Booking, bc.Customer, Car = car })
                .Select(x => new AllBookingsViewModel
                {
                    CustomerFullName = x.Customer.Fullname,
                    CustomerUsername = x.Customer.Username,
                    CustomerEmail = x.Customer.Email,

                    CarBrand = x.Car.Brand,
                    CarModel = x.Car.Model,
                    CarType = x.Car.CarType,
                    CarSeatCapacity = x.Car.SeatCapacity,
                    CarRentPerDay = x.Car.RentPerDay,
                    CarImagePath = x.Car.ImagePath,

                    StartDate = x.Booking.StartDate,
                    EndDate = x.Booking.EndDate,
                    TotalCost = x.Booking.TotalCost,
                    IsActive = x.Booking.EndDate >= DateTime.Now,

                    // ✅ Pull payment info
                    PaymentStatus = _context.Payments
                        .Where(p => p.BookingId == x.Booking.Id)
                        .Select(p => p.Status)
                        .FirstOrDefault() ?? "Not Paid",

                    PaymentDate = _context.Payments
                        .Where(p => p.BookingId == x.Booking.Id)
                        .Select(p => (DateTime?)p.PaymentDate)
                        .FirstOrDefault(),

                    CardLast4 = _context.Payments
                        .Where(p => p.BookingId == x.Booking.Id)
                        .Select(p => p.CardLast4)
                        .FirstOrDefault(),
                })
                .OrderByDescending(b => b.IsActive)   // Active bookings first
                .ThenByDescending(b => b.StartDate)   // Latest bookings first
                .ToList();

            return View(bookings);
        }

    }
}
