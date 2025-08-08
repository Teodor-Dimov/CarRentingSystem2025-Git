using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .Include(b => b.Cars)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Name,Description,Headquarters,FoundedDate,Website,PhoneNumber,Email,Slogan,CEO,Status,IsFeatured")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                brand.CreatedAt = DateTime.Now;
                brand.UpdatedAt = DateTime.Now;
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Headquarters,FoundedDate,Website,PhoneNumber,Email,Slogan,CEO,Status,IsFeatured,CreatedAt")] Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    brand.UpdatedAt = DateTime.Now;
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .Include(b => b.Cars)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Cars)
                .ThenInclude(c => c.Rentals)
                .FirstOrDefaultAsync(b => b.Id == id);
                
            if (brand != null)
            {
                // Delete all rentals for all cars of this brand
                var allRentals = brand.Cars?.SelectMany(c => c.Rentals) ?? new List<Rental>();
                if (allRentals.Any())
                {
                    _context.Rentals.RemoveRange(allRentals);
                }
                
                // Delete all cars of this brand
                if (brand.Cars != null && brand.Cars.Any())
                {
                    _context.Cars.RemoveRange(brand.Cars);
                }
                
                // Delete the brand
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Brand '{brand.Name}' and all its {brand.Cars?.Count ?? 0} cars with {allRentals.Count()} rentals have been deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}

