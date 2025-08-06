using CarRentingSystem2025.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (customer == null)
                return View("NoCustomer");

            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .Where(r => r.CustomerId == customer.Id)
                .OrderByDescending(r => r.PickupDate)
                .ToListAsync();

            ViewBag.Customer = customer;
            return View(rentals);
        }
    }
}
