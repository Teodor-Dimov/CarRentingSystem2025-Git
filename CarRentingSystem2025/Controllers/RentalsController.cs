using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRentingSystem2025.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(rentals);
        }

        // GET: Rentals/Create
        public IActionResult Create(int? carId = null)
        {
            ViewData["CarId"] = new SelectList(_context.Cars.Where(c => c.IsAvailable), "Id", "DisplayName", carId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "DisplayName");
            return View();
        }

        // POST: Rentals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarId,CustomerId,PickupDate,DropoffDate,TotalAmount,Status,Notes")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                rental.Status = "Pending";
                rental.CreatedAt = DateTime.Now;
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars.Where(c => c.IsAvailable), "Id", "DisplayName", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "DisplayName", rental.CustomerId);
            return View(rental);
        }
    }
} 