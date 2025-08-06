using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentingSystem2025.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            var brands = await _context.Brands
                .Include(b => b.Cars)
                .ToListAsync();
            return View(brands);
        }
    }
}
