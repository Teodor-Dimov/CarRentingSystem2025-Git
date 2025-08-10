using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;

namespace CarRentingSystem2025.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Rentals
        public async Task<IActionResult> Index()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(rentals);
        }

        // GET: Admin/Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Admin/Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            ViewBag.Cars = await _context.Cars.Include(c => c.BrandEntity).ToListAsync();
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.StatusOptions = new[] { "Active", "Completed", "Cancelled", "Overdue" };

            return View(rental);
        }

        // POST: Admin/Rentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,CustomerId,PickupDate,DropoffDate,TotalAmount,PickupLocation,DropoffLocation,Status,ActualPickupTime,ActualDropoffTime,DepositAmount,InsuranceType,InsuranceCost,LateFees,DamageFees,DamageDescription,SpecialRequests,CustomerRating,CustomerReview,CreatedAt")] Rental rental)
        {
            try
            {
                Console.WriteLine($"Edit POST called with id: {id}");
                Console.WriteLine($"Rental object: {rental?.Id}, CarId: {rental?.CarId}, CustomerId: {rental?.CustomerId}");
                Console.WriteLine($"Form data - CarId: {Request.Form["CarId"]}, CustomerId: {Request.Form["CustomerId"]}");
                Console.WriteLine($"Model binding - CarId: {rental?.CarId}, CustomerId: {rental?.CustomerId}");
                Console.WriteLine($"Form data - PickupDate: {Request.Form["PickupDate"]}, DropoffDate: {Request.Form["DropoffDate"]}");
                Console.WriteLine($"Form data - TotalAmount: {Request.Form["TotalAmount"]}, Status: {Request.Form["Status"]}");
                Console.WriteLine($"Form data - PickupLocation: {Request.Form["PickupLocation"]}, DropoffLocation: {Request.Form["DropoffLocation"]}");
                
                if (id != rental.Id)
                {
                    Console.WriteLine("ID mismatch");
                    return NotFound();
                }

                // For editing existing rentals, remove validation errors for dates that might be in the past
                ModelState.Remove("PickupDate");
                ModelState.Remove("DropoffDate");

                // Validate that CarId and CustomerId exist
                var carExists = await _context.Cars.AnyAsync(c => c.Id == rental.CarId);
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == rental.CustomerId);

                if (!carExists)
                {
                    ModelState.AddModelError("CarId", "Selected car does not exist");
                    Console.WriteLine($"Car with ID {rental.CarId} does not exist");
                }

                if (!customerExists)
                {
                    ModelState.AddModelError("CustomerId", "Selected customer does not exist");
                    Console.WriteLine($"Customer with ID {rental.CustomerId} does not exist");
                }

                if (ModelState.IsValid && carExists && customerExists)
                {
                    Console.WriteLine("ModelState is valid, updating rental...");
                    
                    // Load the existing rental from database
                    var existingRental = await _context.Rentals.FindAsync(id);
                    if (existingRental == null)
                    {
                        Console.WriteLine("Existing rental not found");
                        return NotFound();
                    }
                    
                    // Update only the properties that were changed
                    existingRental.CarId = rental.CarId;
                    existingRental.CustomerId = rental.CustomerId;
                    existingRental.PickupDate = rental.PickupDate;
                    existingRental.DropoffDate = rental.DropoffDate;
                    existingRental.TotalAmount = rental.TotalAmount;
                    existingRental.Status = rental.Status;
                    existingRental.PickupLocation = rental.PickupLocation;
                    existingRental.DropoffLocation = rental.DropoffLocation;
                    existingRental.ActualPickupTime = rental.ActualPickupTime;
                    existingRental.ActualDropoffTime = rental.ActualDropoffTime;
                    existingRental.DepositAmount = rental.DepositAmount;
                    existingRental.InsuranceType = rental.InsuranceType;
                    existingRental.InsuranceCost = rental.InsuranceCost;
                    existingRental.LateFees = rental.LateFees;
                    existingRental.DamageFees = rental.DamageFees;
                    existingRental.DamageDescription = rental.DamageDescription;
                    existingRental.SpecialRequests = rental.SpecialRequests;
                    existingRental.CustomerRating = rental.CustomerRating;
                    existingRental.CustomerReview = rental.CustomerReview;
                    existingRental.UpdatedAt = DateTime.Now;
                    
                    Console.WriteLine($"Updated values - CarId: {existingRental.CarId}, CustomerId: {existingRental.CustomerId}");
                    Console.WriteLine($"Updated dates - Pickup: {existingRental.PickupDate}, Dropoff: {existingRental.DropoffDate}");
                    Console.WriteLine($"Updated amount: {existingRental.TotalAmount}");
                    
                    _context.Update(existingRental);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Rental updated successfully");
                    TempData["SuccessMessage"] = "Rental updated successfully!";
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
                TempData["ErrorMessage"] = "An error occurred while updating the rental.";
            }

            ViewBag.Cars = await _context.Cars.Include(c => c.BrandEntity).ToListAsync();
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.StatusOptions = new[] { "Active", "Completed", "Cancelled", "Overdue" };

            return View(rental);
        }

        // GET: Admin/Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Console.WriteLine($"===== ADMIN RENTALS DELETE ACTION CALLED =====");
            Console.WriteLine($"Admin Rentals Delete action called with id: {id}");
            
            if (id == null)
            {
                Console.WriteLine("Admin Rentals Delete: id is null - returning NotFound");
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                Console.WriteLine($"Admin Rentals Delete: rental with id {id} not found - returning NotFound");
                return NotFound();
            }

            Console.WriteLine($"Admin Rentals Delete: found rental {rental.Id} for {rental.Customer?.FirstName} {rental.Customer?.LastName}");
            Console.WriteLine("Admin Rentals Delete: returning Delete confirmation view");
            return View(rental);
        }

        // POST: Admin/Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (rental != null)
            {
                // If rental is active, make the car available again
                if (rental.Status == "Active" && rental.Car != null)
                {
                    rental.Car.IsAvailable = true;
                }
                
                // Store info for success message
                var carInfo = $"{rental.Car?.Brand} {rental.Car?.Model}";
                var customerInfo = $"{rental.Customer?.FirstName} {rental.Customer?.LastName}";
                
                _context.Rentals.Remove(rental);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Rental for {carInfo} by {customerInfo} has been deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Rentals/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            rental.Status = newStatus;
            rental.UpdatedAt = DateTime.Now;

            // Update car availability if rental is completed or cancelled
            if (newStatus == "Completed" || newStatus == "Cancelled")
            {
                var car = await _context.Cars.FindAsync(rental.CarId);
                if (car != null)
                {
                    car.IsAvailable = true;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}
