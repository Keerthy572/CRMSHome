using CRMSHome.Data;
using CRMSHome.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRMSHome.Controllers
{
    public class FiltersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FiltersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all filters
        public IActionResult Index()
        {
            var filters = _context.Filters.ToList();
            return View(filters);
        }

        // Add Filter (GET)
        public IActionResult AddFilter()
        {
            return View();
        }

        // Add Filter (POST)
        [HttpPost]
        public IActionResult AddFilter(Filter filter)
        {
            if (ModelState.IsValid)
            {
                filter.Id = Guid.NewGuid();
                _context.Filters.Add(filter);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(filter);
        }

        // Delete Filter
        public IActionResult DeleteFilter(Guid id)
        {
            var filter = _context.Filters.FirstOrDefault(f => f.Id == id);
            if (filter != null)
            {
                _context.Filters.Remove(filter);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
