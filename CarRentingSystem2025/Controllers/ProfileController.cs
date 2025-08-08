using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CarRentingSystem2025.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var customer = await _context.Customers
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                // Create a basic customer profile for the user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    customer = new Customer
                    {
                        UserId = currentUser.Id,
                        FirstName = currentUser.UserName?.Split('@')[0] ?? "User",
                        LastName = "",
                        Email = currentUser.Email ?? "",
                        CreatedAt = DateTime.Now,
                        IsVerified = false
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                    
                    TempData["InfoMessage"] = "Profile created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            // Calculate statistics
            ViewBag.TotalRentals = customer.Rentals.Count;
            ViewBag.CompletedRentals = customer.Rentals.Count(r => r.Status == "Completed");
            ViewBag.ActiveRentals = customer.Rentals.Count(r => r.Status == "Active");
            ViewBag.TotalSpent = customer.Rentals.Sum(r => r.TotalAmount);

            return View(customer);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                // Create a basic customer profile for the user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    customer = new Customer
                    {
                        UserId = currentUser.Id,
                        FirstName = currentUser.UserName?.Split('@')[0] ?? "User",
                        LastName = "",
                        Email = currentUser.Email ?? "",
                        CreatedAt = DateTime.Now,
                        IsVerified = false
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,UserId,FirstName,LastName,Email,PhoneNumber,Address,City,PostalCode,Country,DriverLicenseNumber,DriverLicenseExpiry,DateOfBirth,EmergencyContactName,EmergencyContactPhone,InsuranceProvider,InsurancePolicyNumber,InsuranceExpiryDate,MembershipLevel,Notes")] Customer customer)
        {
            var userId = _userManager.GetUserId(User);
            var currentCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (currentCustomer == null || currentCustomer.Id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.UpdatedAt = DateTime.Now;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> MyCars()
        {
            var userId = _userManager.GetUserId(User);
            var customer = await _context.Customers
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Please complete your profile first.";
                return RedirectToAction("Index");
            }

            // Get all cars that the user has rented
            var userCars = customer.Rentals
                .Select(r => r.Car)
                .Distinct()
                .ToList();

            return View(userCars);
        }

        public async Task<IActionResult> CarDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var customer = await _context.Customers
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound();
            }

            // Check if the user has rented this car
            var car = customer.Rentals
                .Where(r => r.Car.Id == id)
                .Select(r => r.Car)
                .FirstOrDefault();

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
