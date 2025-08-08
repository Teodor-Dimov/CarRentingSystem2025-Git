using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarRentingSystem2025.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 6; // Number of cars per page

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string searchString, string brandFilter, string priceFilter, 
            string availabilityFilter, string bodyTypeFilter, string transmissionFilter, string fuelTypeFilter, 
            decimal? minPrice, decimal? maxPrice, DateTime? pickupDate, DateTime? dropoffDate, int page = 1)
        {
            var cars = _context.Cars.Include(c => c.BrandEntity).AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c =>
                    c.Brand.Contains(searchString) ||
                    c.Model.Contains(searchString) ||
                    c.LicensePlate.Contains(searchString) ||
                    c.BrandEntity.Name.Contains(searchString) ||
                    c.Features.Contains(searchString));
            }

            // Brand filter
            if (!string.IsNullOrEmpty(brandFilter))
            {
                cars = cars.Where(c => c.BrandEntity.Name == brandFilter);
            }

            // Price filter
            if (!string.IsNullOrEmpty(priceFilter))
            {
                switch (priceFilter)
                {
                    case "low":
                        cars = cars.Where(c => c.DailyRate <= 50);
                        break;
                    case "medium":
                        cars = cars.Where(c => c.DailyRate > 50 && c.DailyRate <= 100);
                        break;
                    case "high":
                        cars = cars.Where(c => c.DailyRate > 100);
                        break;
                }
            }

            // Custom price range
            if (minPrice.HasValue)
            {
                cars = cars.Where(c => c.DailyRate >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                cars = cars.Where(c => c.DailyRate <= maxPrice.Value);
            }

            // Availability filter
            if (!string.IsNullOrEmpty(availabilityFilter))
            {
                switch (availabilityFilter)
                {
                    case "available":
                        cars = cars.Where(c => c.IsAvailable);
                        break;
                    case "unavailable":
                        cars = cars.Where(c => !c.IsAvailable);
                        break;
                }
            }

            // Body type filter
            if (!string.IsNullOrEmpty(bodyTypeFilter))
            {
                cars = cars.Where(c => c.BodyType == bodyTypeFilter);
            }

            // Transmission filter
            if (!string.IsNullOrEmpty(transmissionFilter))
            {
                cars = cars.Where(c => c.Transmission == transmissionFilter);
            }

            // Fuel type filter
            if (!string.IsNullOrEmpty(fuelTypeFilter))
            {
                cars = cars.Where(c => c.FuelType == fuelTypeFilter);
            }

            // Date availability filter
            if (pickupDate.HasValue && dropoffDate.HasValue)
            {
                // Get cars that are not rented during the specified period
                var rentedCarIds = await _context.Rentals
                    .Where(r => (r.PickupDate <= dropoffDate.Value && r.DropoffDate >= pickupDate.Value) &&
                               (r.Status == "Active" || r.Status == "Pending"))
                    .Select(r => r.CarId)
                    .ToListAsync();

                cars = cars.Where(c => !rentedCarIds.Contains(c.Id));
            }

            // Get total count for pagination
            var totalCars = await cars.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCars / PageSize);

            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

            // Apply pagination
            var carsList = await cars
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Get filter options for dropdowns
            var brands = await _context.Brands.Select(b => b.Name).Distinct().ToListAsync();
            var bodyTypes = await _context.Cars.Select(c => c.BodyType).Distinct().Where(bt => !string.IsNullOrEmpty(bt)).ToListAsync();
            var transmissionTypes = await _context.Cars.Select(c => c.Transmission).Distinct().Where(t => !string.IsNullOrEmpty(t)).ToListAsync();
            var fuelTypes = await _context.Cars.Select(c => c.FuelType).Distinct().Where(ft => !string.IsNullOrEmpty(ft)).ToListAsync();

            // Set ViewBag values
            ViewBag.SearchString = searchString;
            ViewBag.BrandFilter = brandFilter;
            ViewBag.PriceFilter = priceFilter;
            ViewBag.AvailabilityFilter = availabilityFilter;
            ViewBag.BodyTypeFilter = bodyTypeFilter;
            ViewBag.TransmissionFilter = transmissionFilter;
            ViewBag.FuelTypeFilter = fuelTypeFilter;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.PickupDate = pickupDate;
            ViewBag.DropoffDate = dropoffDate;
            ViewBag.Brands = brands;
            ViewBag.BodyTypes = bodyTypes;
            ViewBag.TransmissionTypes = transmissionTypes;
            ViewBag.FuelTypes = fuelTypes;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCars = totalCars;
            ViewBag.PageSize = PageSize;

            return View(carsList);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            // Get rental history for this car
            var rentalHistory = await _context.Rentals
                .Where(r => r.CarId == id)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.PickupDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RentalHistory = rentalHistory;

            return View(car);
        }

        // GET: Cars/Search
        public async Task<IActionResult> Search()
        {
            // Get filter options
            var brands = await _context.Brands.Select(b => b.Name).Distinct().ToListAsync();
            var bodyTypes = await _context.Cars.Select(c => c.BodyType).Distinct().Where(bt => !string.IsNullOrEmpty(bt)).ToListAsync();
            var transmissionTypes = await _context.Cars.Select(c => c.Transmission).Distinct().Where(t => !string.IsNullOrEmpty(t)).ToListAsync();
            var fuelTypes = await _context.Cars.Select(c => c.FuelType).Distinct().Where(ft => !string.IsNullOrEmpty(ft)).ToListAsync();

            ViewBag.Brands = brands;
            ViewBag.BodyTypes = bodyTypes;
            ViewBag.TransmissionTypes = transmissionTypes;
            ViewBag.FuelTypes = fuelTypes;

            return View();
        }

        // GET: Cars/Featured
        public async Task<IActionResult> Featured()
        {
            var featuredCars = await _context.Cars
                .Include(c => c.BrandEntity)
                .Where(c => c.IsFeatured && c.IsAvailable)
                .OrderByDescending(c => c.Rating)
                .Take(6)
                .ToListAsync();

            return View(featuredCars);
        }

        // GET: Cars/Popular
        public async Task<IActionResult> Popular()
        {
            var popularCars = await _context.Cars
                .Include(c => c.BrandEntity)
                .Where(c => c.IsAvailable)
                .OrderByDescending(c => c.NumberOfRentals)
                .Take(10)
                .ToListAsync();

            return View(popularCars);
        }
    }
}