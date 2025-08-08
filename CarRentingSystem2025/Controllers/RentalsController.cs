using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CarRentingSystem2025.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RentalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Rentals
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrator"))
            {
                // Admin sees all rentals
                var rentals = await _context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.Customer)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                return View(rentals);
            }
                         else
             {
                                   // Regular users see only their own rentals
                  var currentUser = await _userManager.GetUserAsync(User);
                  var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == currentUser!.Id);
                 
                 if (customer == null)
                 {
                     // Create a basic customer profile for the user
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
                         return RedirectToAction("Index", "Profile");
                     }
                     else
                     {
                         return RedirectToAction("Login", "Account");
                     }
                 }

                var rentals = await _context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.Customer)
                    .Where(r => r.CustomerId == customer.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                return View(rentals);
            }
        }

        // GET: Rentals/Create
        [Authorize]
        public async Task<IActionResult> Create(int? carId = null)
        {
            // Get available cars
            var availableCars = _context.Cars
                .Where(c => c.IsAvailable)
                .Include(c => c.BrandEntity)
                .ToList();

            // Get current user's customer profile
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
                         var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == currentUser!.Id);
            
            if (customer == null)
            {
                // Create a basic customer profile for the user
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
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            ViewData["CarId"] = new SelectList(availableCars.Select(c => new { c.Id, DisplayName = $"{c.DisplayName} - ${c.DailyRate:F2}/day" }), "Id", "DisplayName", carId);
            ViewBag.CustomerId = customer.Id; // Set the current user's customer ID

            // Pre-populate with selected car
            if (carId.HasValue)
            {
                var selectedCar = availableCars.FirstOrDefault(c => c.Id == carId.Value);
                if (selectedCar != null)
                {
                    ViewBag.SelectedCar = selectedCar;
                }
            }

            return View();
        }

        // POST: Rentals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("CarId,PickupDate,DropoffDate,PickupLocation,DropoffLocation,SpecialRequests")] Rental rental)
        {
            Console.WriteLine("=== CREATE POST ACTION CALLED ===");
            Console.WriteLine($"Request received at: {DateTime.Now}");
            Console.WriteLine($"User: {User.Identity?.Name}");
            Console.WriteLine($"Is Authenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"CarId: {rental.CarId}");
            Console.WriteLine($"PickupDate: {rental.PickupDate}");
            Console.WriteLine($"DropoffDate: {rental.DropoffDate}");
            Console.WriteLine($"TotalAmount: {rental.TotalAmount}");
            Console.WriteLine($"PickupLocation: {rental.PickupLocation}");
            Console.WriteLine($"DropoffLocation: {rental.DropoffLocation}");
            Console.WriteLine($"SpecialRequests: {rental.SpecialRequests}");
            
                         Console.WriteLine("Getting current user...");
             // Get current user's customer profile
             var currentUser = await _userManager.GetUserAsync(User);
             Console.WriteLine($"Current user retrieved: {currentUser?.UserName}");
             
             Console.WriteLine("Getting customer profile...");
             var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == currentUser!.Id);
             Console.WriteLine($"Customer profile retrieved: {customer?.Id}");
            
            if (customer == null)
            {
                // Create a basic customer profile for the user
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
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

                         Console.WriteLine("Entering try block...");
             try
             {
                 Console.WriteLine("Setting customer ID...");
                 // Set the customer ID automatically
                 rental.CustomerId = customer.Id;

                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                Console.WriteLine($"ModelState Error Count: {ModelState.ErrorCount}");
                
                // Additional validation
                if (rental.PickupDate >= rental.DropoffDate)
                {
                    ModelState.AddModelError("DropoffDate", "Dropoff date must be after pickup date.");
                }
                
                if (rental.PickupDate < DateTime.Now.Date)
                {
                    ModelState.AddModelError("PickupDate", "Pickup date cannot be in the past.");
                }
                
                if ((rental.DropoffDate - rental.PickupDate).Days > 30)
                {
                    ModelState.AddModelError("DropoffDate", "Rental duration cannot exceed 30 days.");
                }
                
                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid, proceeding with rental creation");
                    // Get the car to calculate total amount
                    var car = await _context.Cars.FindAsync(rental.CarId);
                    if (car != null)
                    {
                        // Calculate total amount based on rental duration
                        var rentalDuration = (rental.DropoffDate - rental.PickupDate).Days;
                        rental.TotalAmount = car.DailyRate * rentalDuration;
                        Console.WriteLine($"Calculated TotalAmount: {rental.TotalAmount}");
                    }
                    else
                    {
                        Console.WriteLine("Car not found, cannot calculate total amount");
                        ModelState.AddModelError("CarId", "Selected car not found");
                        return View(rental);
                    }

                    // Set default values
                    rental.Status = "Active";
                    rental.CreatedAt = DateTime.Now;

                    _context.Add(rental);
                    await _context.SaveChangesAsync();

                    // Update car availability
                    if (car != null)
                    {
                        car.IsAvailable = false;
                        car.NumberOfRentals++;
                        await _context.SaveChangesAsync();
                    }

                    Console.WriteLine("Rental created successfully, redirecting...");
                    TempData["SuccessMessage"] = "Rental created successfully!";
                    
                    // Redirect based on user role
                    if (User.IsInRole("Administrator"))
                    {
                        Console.WriteLine("Redirecting admin to Admin Rentals");
                        return RedirectToAction("Index", "Rentals", new { area = "Admin" });
                    }
                    else
                    {
                        Console.WriteLine("Redirecting user to Rentals Index");
                        return RedirectToAction("Index", "Rentals");
                    }
                }
                else
                {
                    Console.WriteLine("=== MODELSTATE IS NOT VALID ===");
                    // Log validation errors
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                    
                    // Re-populate dropdowns on validation error
                    var carsForDropdown = _context.Cars
                        .Where(c => c.IsAvailable)
                        .Include(c => c.BrandEntity)
                        .ToList();

                    ViewData["CarId"] = new SelectList(carsForDropdown.Select(c => new { c.Id, DisplayName = $"{c.DisplayName} - ${c.DailyRate:F2}/day" }), "Id", "DisplayName", rental != null ? rental.CarId : (int?)null);
                    ViewBag.CustomerId = customer?.Id ?? 0;

                    return View(rental);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating rental: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the rental. Please try again.");
            }

            Console.WriteLine("=== EXCEPTION OR ERROR OCCURRED ===");
            // Re-populate dropdowns on validation error
            var carsForException = _context.Cars
                .Where(c => c.IsAvailable)
                .Include(c => c.BrandEntity)
                .ToList();

            ViewData["CarId"] = new SelectList(carsForException.Select(c => new { c.Id, DisplayName = $"{c.DisplayName} - ${c.DailyRate:F2}/day" }), "Id", "DisplayName", rental != null ? rental.CarId : (int?)null);
            ViewBag.CustomerId = customer?.Id ?? 0;

            return View(rental);
        }

        // GET: Rentals/Details/5
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

        // GET: Rentals/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "DisplayName", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "DisplayName", rental.CustomerId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,CustomerId,PickupDate,DropoffDate,TotalAmount,Status,PickupLocation,DropoffLocation,SpecialRequests")] Rental rental)
        {
            if (id != rental.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRental = await _context.Rentals.FindAsync(id);
                    if (existingRental == null)
                    {
                        return NotFound();
                    }

                    // Update rental properties
                    existingRental.CarId = rental.CarId;
                    existingRental.CustomerId = rental.CustomerId;
                    existingRental.PickupDate = rental.PickupDate;
                    existingRental.DropoffDate = rental.DropoffDate;
                    existingRental.TotalAmount = rental.TotalAmount;
                    existingRental.Status = rental.Status;
                    existingRental.PickupLocation = rental.PickupLocation;
                    existingRental.DropoffLocation = rental.DropoffLocation;
                    existingRental.SpecialRequests = rental.SpecialRequests;

                    _context.Update(existingRental);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Rental updated successfully!";
                    return RedirectToAction(nameof(Index));
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
            }

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "DisplayName", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "DisplayName", rental.CustomerId);
            return View(rental);
        }

        // POST: Rentals/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            rental.Status = status;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Rental status updated to {status}!";
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
} 