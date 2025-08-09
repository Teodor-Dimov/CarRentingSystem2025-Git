using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;

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

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Profile Index action called");
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                Console.WriteLine("Profile Index: User not found");
                return RedirectToAction("Login", "Account");
            }

            Console.WriteLine($"Profile Index: Current user ID: {currentUser.Id}");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == currentUser.Id);

            if (customer == null)
            {
                Console.WriteLine("Profile Index: Customer profile not found, creating new one");
                // Create a basic customer profile for the user
                customer = new Customer
                {
                    UserId = currentUser.Id,
                    FirstName = currentUser.UserName?.Split('@')[0] ?? "User",
                    LastName = "",
                    Email = currentUser.Email ?? "",
                    PhoneNumber = "",
                    DriversLicenseNumber = "",
                    Address = "",
                    City = "",
                    Country = "",
                    PostalCode = "",
                    DateOfBirth = DateTime.Now.AddYears(-25), // Default age
                    MembershipStartDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    IsVerified = false
                };
                
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"Profile Index: Created new customer profile with ID: {customer.Id}");
                TempData["SuccessMessage"] = "Profile created successfully! Please complete your profile information.";
            }
            else
            {
                Console.WriteLine($"Profile Index: Found existing customer profile with ID: {customer.Id}");
            }

            // Load rental statistics for the customer
            var rentals = await _context.Rentals
                .Where(r => r.CustomerId == customer.Id)
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .ToListAsync();

            customer.Rentals = rentals;

            // Calculate statistics
            ViewBag.TotalRentals = rentals.Count;
            ViewBag.CompletedRentals = rentals.Count(r => r.Status == "Completed");
            ViewBag.ActiveRentals = rentals.Count(r => r.Status == "Active");
            ViewBag.TotalSpent = rentals.Where(r => r.Status == "Completed").Sum(r => r.TotalAmount);

            Console.WriteLine($"Profile Index: Loaded {rentals.Count} rentals for customer");

            return View(customer);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            Console.WriteLine("Profile Edit GET action called");
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                Console.WriteLine("Profile Edit: User not found");
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == currentUser.Id);

            if (customer == null)
            {
                Console.WriteLine("Profile Edit: Customer profile not found, redirecting to Index");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"Profile Edit: Found customer profile with ID: {customer.Id}");
            return View(customer);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,FirstName,LastName,Email,PhoneNumber,DriversLicenseNumber,Address,City,Country,PostalCode,DateOfBirth")] Customer customer)
        {
            Console.WriteLine($"Profile Edit POST action called for customer ID: {customer.Id}");
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                Console.WriteLine("Profile Edit POST: User not found");
                return RedirectToAction("Login", "Account");
            }

            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == currentUser.Id && c.Id == customer.Id);

            if (existingCustomer == null)
            {
                Console.WriteLine("Profile Edit POST: Customer profile not found or unauthorized");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update only the fields that can be edited
                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.DriversLicenseNumber = customer.DriversLicenseNumber;
                    existingCustomer.Address = customer.Address;
                    existingCustomer.City = customer.City;
                    existingCustomer.Country = customer.Country;
                    existingCustomer.PostalCode = customer.PostalCode;
                    existingCustomer.DateOfBirth = customer.DateOfBirth;
                    existingCustomer.UpdatedAt = DateTime.Now;

                    _context.Update(existingCustomer);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Profile Edit POST: Successfully updated customer profile ID: {customer.Id}");
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Profile Edit POST: Concurrency exception: {ex.Message}");
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Profile Edit POST: Exception: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while updating your profile. Please try again.";
                }
            }
            else
            {
                Console.WriteLine("Profile Edit POST: Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model validation error: {error.ErrorMessage}");
                }
            }

            return View(customer);
        }

        // GET: Profile/MyCars
        public async Task<IActionResult> MyCars()
        {
            Console.WriteLine("Profile MyCars action called");
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                Console.WriteLine("Profile MyCars: User not found");
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == currentUser.Id);

            if (customer == null)
            {
                Console.WriteLine("Profile MyCars: Customer profile not found, redirecting to Index");
                return RedirectToAction(nameof(Index));
            }

            // For now, redirect to rentals as "MyCars" would be the cars they've rented
            Console.WriteLine("Profile MyCars: Redirecting to Rentals");
            return RedirectToAction("Index", "Rentals");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
