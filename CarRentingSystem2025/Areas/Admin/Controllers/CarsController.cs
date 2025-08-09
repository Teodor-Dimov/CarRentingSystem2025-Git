using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentingSystem2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Temporarily removed Roles = "Administrator" for testing
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Cars
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars
                .Include(c => c.BrandEntity)
                .Include(c => c.Rentals.Where(r => r.Status == "Active"))
                .ThenInclude(r => r.Customer)
                .ToListAsync();

            return View(cars);
        }

        // GET: Admin/Cars/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.BrandId = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Admin/Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrandId,Brand,Model,Year,LicensePlate,DailyRate,Color,FuelType,Transmission,Seats,Mileage,EngineSize,BodyType,DriveType,ReleaseDate,LastServiceDate,NextServiceDate,Features,Description,ImageUrl,IsAvailable,IsFeatured")] Car car)
        {
            if (ModelState.IsValid)
            {
                car.CreatedAt = DateTime.Now;
                _context.Add(car);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Car created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BrandId = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", car.BrandId);
            return View(car);
        }

        // GET: Admin/Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Console.WriteLine($"===== ADMIN CARS EDIT ACTION CALLED =====");
            Console.WriteLine($"Edit action called with id: {id}");
            Console.WriteLine($"Area: {ControllerContext.ActionDescriptor.RouteValues["area"]}");
            Console.WriteLine($"Controller: {ControllerContext.ActionDescriptor.RouteValues["controller"]}");
            Console.WriteLine($"Action: {ControllerContext.ActionDescriptor.RouteValues["action"]}");
            
            if (id == null)
            {
                Console.WriteLine("Edit action: id is null - returning NotFound");
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (car == null)
            {
                Console.WriteLine($"Edit action: car with id {id} not found - returning NotFound");
                return NotFound();
            }
            
            Console.WriteLine($"Edit action: found car {car.Brand} {car.Model} (ID: {car.Id})");
            ViewBag.BrandId = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", car.BrandId);
            Console.WriteLine("Edit action: returning Edit view");
            return View(car);
        }

        // POST: Admin/Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BrandId,Brand,Model,Year,LicensePlate,DailyRate,Color,FuelType,Transmission,Seats,Mileage,EngineSize,BodyType,DriveType,ReleaseDate,LastServiceDate,NextServiceDate,Features,Description,ImageUrl,IsAvailable,IsFeatured,Rating,NumberOfRentals,CreatedAt,UpdatedAt")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            // Log validation errors for debugging
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the brand name from the selected brand
                    var brand = await _context.Brands.FindAsync(car.BrandId);
                    if (brand != null)
                    {
                        car.Brand = brand.Name;
                    }
                    
                    car.UpdatedAt = DateTime.Now;
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Car updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
            ViewBag.BrandId = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", car.BrandId);
            return View(car);
        }

        // GET: Admin/Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .Include(c => c.Rentals)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Admin/Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Rentals)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (car != null)
            {
                // Delete all rentals for this car
                if (car.Rentals != null && car.Rentals.Any())
                {
                    _context.Rentals.RemoveRange(car.Rentals);
                }
                
                // Delete the car
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Car '{car.Brand} {car.Model}' and all its {car.Rentals?.Count ?? 0} rentals have been deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
