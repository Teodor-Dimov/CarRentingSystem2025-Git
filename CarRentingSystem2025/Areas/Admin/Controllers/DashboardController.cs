using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;

namespace CarRentingSystem2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get statistics
            var totalCars = await _context.Cars.CountAsync();
            var availableCars = await _context.Cars.Where(c => c.IsAvailable).CountAsync();
            var activeRentals = await _context.Rentals.Where(r => r.Status == "Active").CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();

            // Get all rentals
            var recentRentals = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // Get all cars
            var availableCarsList = await _context.Cars
                .Include(c => c.BrandEntity)
                .ToListAsync();

            ViewBag.TotalCars = totalCars;
            ViewBag.AvailableCars = availableCars;
            ViewBag.ActiveRentals = activeRentals;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.RecentRentals = recentRentals;
            ViewBag.AvailableCarsList = availableCarsList;

            return View();
        }
    }
}

