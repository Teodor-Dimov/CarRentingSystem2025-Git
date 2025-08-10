using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentingSystem2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
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
            var brands = await _context.Brands.ToListAsync();
            ViewBag.BrandId = new SelectList(brands, "Id", "Name");
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
            
            var brands = await _context.Brands.ToListAsync();
            Console.WriteLine($"POST Create: Repopulating {brands.Count} brands due to validation error");
            ViewBag.BrandId = new SelectList(brands, "Id", "Name", car.BrandId);
            return View(car);
        }

        // GET: Admin/Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            
            ViewBag.BrandId = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", car.BrandId);
            return View(car);
        }

        // POST: Admin/Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BrandId,Brand,Model,Year,LicensePlate,DailyRate,Color,FuelType,Transmission,Seats,Mileage,EngineSize,BodyType,DriveType,ReleaseDate,LastServiceDate,NextServiceDate,Features,Description,ImageUrl,IsAvailable,IsFeatured,Rating,NumberOfRentals,CreatedAt,UpdatedAt")] Car car)
        {
            try
            {
                Console.WriteLine($"Edit POST called with id: {id}");
                Console.WriteLine($"Car object: {car?.Id}, BrandId: {car?.BrandId}, Model: {car?.Model}");
                Console.WriteLine($"Form data - BrandId: {Request.Form["BrandId"]}, Model: {Request.Form["Model"]}");
                Console.WriteLine($"Form data - Year: {Request.Form["Year"]}, DailyRate: {Request.Form["DailyRate"]}");
                Console.WriteLine($"Form data - IsAvailable: {Request.Form["IsAvailable"]}, IsFeatured: {Request.Form["IsFeatured"]}");
                Console.WriteLine($"Form data - LicensePlate: {Request.Form["LicensePlate"]}");
                
                if (id != car.Id)
                {
                    Console.WriteLine("ID mismatch");
                    return NotFound();
                }

                // Validate that BrandId exists
                var brandExists = await _context.Brands.AnyAsync(b => b.Id == car.BrandId);
                if (!brandExists)
                {
                    ModelState.AddModelError("BrandId", "Selected brand does not exist");
                    Console.WriteLine($"Brand with ID {car.BrandId} does not exist");
                }

                // Remove validation errors for fields that we handle manually
                ModelState.Remove("Brand");
                ModelState.Remove("BrandEntity");
                ModelState.Remove("LicensePlate");
                
                if (ModelState.IsValid && brandExists)
                {
                    Console.WriteLine("ModelState is valid, updating car...");
                    
                    // Load the existing car from database
                    var existingCar = await _context.Cars.FindAsync(id);
                    if (existingCar == null)
                    {
                        Console.WriteLine("Existing car not found");
                        return NotFound();
                    }
                    
                    // Update only the properties that were changed
                    existingCar.BrandId = car.BrandId;
                    existingCar.Model = car.Model;
                    existingCar.Year = car.Year;
                    existingCar.LicensePlate = car.LicensePlate;
                    existingCar.DailyRate = car.DailyRate;
                    existingCar.Color = car.Color;
                    existingCar.FuelType = car.FuelType;
                    existingCar.Transmission = car.Transmission;
                    existingCar.Seats = car.Seats;
                    existingCar.Mileage = car.Mileage;
                    existingCar.EngineSize = car.EngineSize;
                    existingCar.BodyType = car.BodyType;
                    existingCar.DriveType = car.DriveType;
                    existingCar.ReleaseDate = car.ReleaseDate;
                    existingCar.LastServiceDate = car.LastServiceDate;
                    existingCar.NextServiceDate = car.NextServiceDate;
                    existingCar.Features = car.Features;
                    existingCar.Description = car.Description;
                    existingCar.ImageUrl = car.ImageUrl;
                    existingCar.IsAvailable = car.IsAvailable;
                    existingCar.IsFeatured = car.IsFeatured;
                    existingCar.Rating = car.Rating;
                    existingCar.UpdatedAt = DateTime.Now;
                    
                    // Get the brand name from the selected brand
                    var brand = await _context.Brands.FindAsync(car.BrandId);
                    if (brand != null)
                    {
                        existingCar.Brand = brand.Name;
                    }
                    
                    Console.WriteLine($"Updated values - BrandId: {existingCar.BrandId}, Model: {existingCar.Model}");
                    Console.WriteLine($"Updated rate: {existingCar.DailyRate}");
                    
                    _context.Update(existingCar);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Car updated successfully");
                    TempData["SuccessMessage"] = "Car updated successfully!";
                    Console.WriteLine("Redirecting to Index page...");
                    return RedirectToAction("Index");
                }
                else
                {
                    Console.WriteLine("ModelState is invalid:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Edit POST: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "An error occurred while updating the car.";
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
            Console.WriteLine($"===== ADMIN CARS DETAILS ACTION CALLED =====");
            Console.WriteLine($"Details action called with id: {id}");
            
            if (id == null)
            {
                Console.WriteLine("Details action: id is null - returning NotFound");
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                Console.WriteLine($"Details action: car with id {id} not found - returning NotFound");
                return NotFound();
            }

            Console.WriteLine($"Details action: found car {car.Brand} {car.Model} (ID: {car.Id})");
            Console.WriteLine("Details action: returning Details view");
            return View(car);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
