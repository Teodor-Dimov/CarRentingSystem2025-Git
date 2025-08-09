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
            Console.WriteLine($"===== ADMIN RENTALS DETAILS ACTION CALLED =====");
            Console.WriteLine($"Admin Rentals Details action called with id: {id}");
            Console.WriteLine($"Area: {ControllerContext.ActionDescriptor.RouteValues["area"]}");
            Console.WriteLine($"Controller: {ControllerContext.ActionDescriptor.RouteValues["controller"]}");
            Console.WriteLine($"Action: {ControllerContext.ActionDescriptor.RouteValues["action"]}");
            
            if (id == null)
            {
                Console.WriteLine("Admin Rentals Details: id is null - returning NotFound");
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                Console.WriteLine($"Admin Rentals Details: rental with id {id} not found - returning NotFound");
                return NotFound();
            }

            Console.WriteLine($"Admin Rentals Details: found rental {rental.Id} for {rental.Customer?.FirstName} {rental.Customer?.LastName}");
            Console.WriteLine($"Rental details: {rental.Car?.Brand} {rental.Car?.Model} from {rental.PickupDate:yyyy-MM-dd} to {rental.DropoffDate:yyyy-MM-dd}");
            Console.WriteLine("Admin Rentals Details: returning Details view");
            return View(rental);
        }

        // GET: Admin/Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Console.WriteLine($"===== ADMIN RENTALS EDIT ACTION CALLED =====");
            Console.WriteLine($"Admin Rentals Edit action called with id: {id}");
            Console.WriteLine($"Area: {ControllerContext.ActionDescriptor.RouteValues["area"]}");
            Console.WriteLine($"Controller: {ControllerContext.ActionDescriptor.RouteValues["controller"]}");
            Console.WriteLine($"Action: {ControllerContext.ActionDescriptor.RouteValues["action"]}");
            
            if (id == null)
            {
                Console.WriteLine("Admin Rentals Edit: id is null - returning NotFound");
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                Console.WriteLine($"Admin Rentals Edit: rental with id {id} not found - returning NotFound");
                return NotFound();
            }

            Console.WriteLine($"Admin Rentals Edit: found rental {rental.Id} for {rental.Customer?.FirstName} {rental.Customer?.LastName}");
            Console.WriteLine($"Rental details: {rental.Car?.Brand} {rental.Car?.Model} from {rental.PickupDate:yyyy-MM-dd} to {rental.DropoffDate:yyyy-MM-dd}");
            
            var cars = await _context.Cars.ToListAsync();
            var customers = await _context.Customers.ToListAsync();
            Console.WriteLine($"Loaded {cars.Count} cars and {customers.Count} customers for dropdown");
            
            ViewBag.Cars = cars;
            ViewBag.Customers = customers;
            ViewBag.StatusOptions = new[] { "Active", "Completed", "Cancelled", "Overdue" };

            Console.WriteLine("Admin Rentals Edit: returning Edit view");
            return View(rental);
        }

        // POST: Admin/Rentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,CustomerId,PickupDate,DropoffDate,TotalAmount,PickupLocation,DropoffLocation,Status,ActualPickupTime,ActualDropoffTime,DepositAmount,InsuranceType,InsuranceCost,LateFees,DamageFees,DamageDescription,SpecialRequests,CustomerRating,CustomerReview,UpdatedAt")] Rental rental)
        {
            if (id != rental.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    rental.UpdatedAt = DateTime.Now;
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.Id))
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

            ViewBag.Cars = await _context.Cars.ToListAsync();
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
