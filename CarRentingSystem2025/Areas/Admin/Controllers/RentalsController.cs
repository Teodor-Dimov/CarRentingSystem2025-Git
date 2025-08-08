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
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            ViewBag.Cars = await _context.Cars.ToListAsync();
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.StatusOptions = new[] { "Active", "Completed", "Cancelled", "Overdue" };

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

        // POST: Admin/Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
                await _context.SaveChangesAsync();
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
