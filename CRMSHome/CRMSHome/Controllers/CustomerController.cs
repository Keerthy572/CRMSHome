using Microsoft.AspNetCore.Mvc;
using CRMSHome.Data;
using CRMSHome.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMSHome.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard → Shows all available cars
        public IActionResult CustomerDashboard()
        {
            var cars = _context.Cars
                .Where(c => c.AvailableStatus == "Available")
                .ToList();

            foreach (var car in cars)
            {
                var activeBooking = _context.Bookings
                    .Where(b => b.CarId == car.Id && b.EndDate >= DateTime.Now)
                    .FirstOrDefault();

                car.BookingStatus = activeBooking != null ? "Booked" : "Available";
            }

            return View(cars);
        }

        // Car details page
        public IActionResult Details(Guid id)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null) return NotFound();

            return View(car);
        }

        // GET: BookCar
        public IActionResult BookCar(Guid carId)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == carId);

            if (car == null || car.BookingStatus == "Booked")
            {
                return RedirectToAction("CustomerDashboard"); // prevent booking if already booked
            }

            ViewBag.Car = car;
            return View(new Booking
            {
                CarId = car.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            });
        }

        // POST: BookCar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookCar(Booking booking)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == booking.CarId);
            if (car == null || car.BookingStatus == "Booked")
            {
                ModelState.AddModelError("", "This car is already booked.");
                return RedirectToAction("CustomerDashboard");
            }

            if (booking.EndDate <= booking.StartDate)
                ModelState.AddModelError("EndDate", "End date must be after start date.");

            if ((booking.EndDate - booking.StartDate).TotalDays > 30)
                ModelState.AddModelError("EndDate", "Booking cannot exceed 30 days.");

            if (!ModelState.IsValid)
            {
                ViewBag.Car = car;
                return View(booking);
            }

            // ✅ Assign logged-in customer ID from session
            var customerIdString = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerIdString))
                return RedirectToAction("Index", "Login"); // Not logged in

            booking.CustomerId = Guid.Parse(customerIdString);
            booking.Id = Guid.NewGuid();

            // Calculate total cost
            booking.TotalCost = (decimal)((booking.EndDate - booking.StartDate).TotalDays + 1) * car.RentPerDay;

            _context.Bookings.Add(booking);

            car.BookingStatus = "Booked";
            _context.Cars.Update(car);

            _context.SaveChanges();

            return RedirectToAction("Payment", new { bookingId = booking.Id });
        }

        // Show active bookings of the logged-in customer
        public IActionResult Bookings()
        {
            var customerIdString = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerIdString))
                return RedirectToAction("CustomerDashboard");

            Guid customerId = Guid.Parse(customerIdString);

            var bookings = _context.Bookings
                                   .Where(b => b.CustomerId == customerId && b.EndDate >= DateTime.Now)
                                   .Join(_context.Cars,
                                         b => b.CarId,
                                         c => c.Id,
                                         (b, c) => new
                                         {
                                             Booking = b,
                                             Car = c
                                         })
                                   .ToList();

            return View(bookings);
        }

        // Cancel Booking
        [HttpPost]
        public IActionResult CancelBooking(Guid bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null) return NotFound();

            if (booking.StartDate > DateTime.Now)
            {
                var car = _context.Cars.FirstOrDefault(c => c.Id == booking.CarId);
                if (car != null)
                {
                    car.BookingStatus = "Available";
                    _context.Cars.Update(car);
                }

                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }

            return RedirectToAction("Bookings");
        }

        
        public IActionResult Payment(Guid bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            // prepare view model
            var model = new PaymentViewModel
            {
                BookingId = booking.Id,
                Amount = booking.TotalCost
            };

            return View(model); // returns Payment.cshtml
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayNow(PaymentViewModel model)
        {
            // Remove spaces for validation and storage
            model.CardNumber = model.CardNumber.Replace(" ", "");

            if (!ModelState.IsValid)
            {
                return View("Payment", model); // Return same view with errors
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = model.BookingId,
                Amount = model.Amount,
                CardLast4 = model.CardNumber[^4..], // last 4 digits
                CustomerName = model.CardHolder,
                CardBrand = GetCardBrand(model.CardNumber),
                Status = "Success"
            };

            _context.Payments.Add(payment);

            var booking = _context.Bookings.FirstOrDefault(b => b.Id == model.BookingId);
            if (booking != null)
            {
                booking.IsPaid = true;
                _context.Bookings.Update(booking);
            }
            _context.SaveChanges();

            return RedirectToAction("PaymentSuccess", new { paymentId = payment.Id });
        }


        public IActionResult PaymentSuccess(Guid paymentId)
        {
            var paymentWithBooking = (from p in _context.Payments
                                      join b in _context.Bookings on p.BookingId equals b.Id
                                      join c in _context.Cars on b.CarId equals c.Id
                                      where p.Id == paymentId
                                      select new
                                      {
                                          Payment = p,
                                          Booking = b,
                                          Car = c
                                      }).FirstOrDefault();

            if (paymentWithBooking == null) return NotFound();

            return View(paymentWithBooking);
        }

        // small helper for brand
        private string GetCardBrand(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) return "";
            var cc = cardNumber.Replace(" ", "");
            if (cc.StartsWith("4")) return "Visa";
            if (cc.StartsWith("5")) return "MasterCard";
            if (cc.StartsWith("34") || cc.StartsWith("37")) return "Amex";
            return "Card";
        }

    }
}
