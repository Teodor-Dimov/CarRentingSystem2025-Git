using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using System.Text.Json;

namespace CarRentingSystem2025.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RentalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRentals()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .ToListAsync();
            
            return Ok(rentals);
        }

        // GET: api/rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return Ok(rental);
        }

        // GET: api/rentals/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Rental>>> GetActiveRentals()
        {
            var activeRentals = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .Where(r => r.Status == "Active")
                .ToListAsync();
            
            return Ok(activeRentals);
        }

        // GET: api/rentals/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRentalsByCustomer(int customerId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
            
            return Ok(rentals);
        }

        // GET: api/rentals/car/{carId}
        [HttpGet("car/{carId}")]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRentalsByCar(int carId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.BrandEntity)
                .Include(r => r.Customer)
                .Where(r => r.CarId == carId)
                .ToListAsync();
            
            return Ok(rentals);
        }

        // POST: api/rentals
        [HttpPost]
        public async Task<ActionResult<Rental>> CreateRental(Rental rental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Calculate total amount
            var car = await _context.Cars.FindAsync(rental.CarId);
            if (car != null)
            {
                var rentalDuration = (rental.DropoffDate - rental.PickupDate).Days;
                rental.TotalAmount = car.DailyRate * rentalDuration;
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
        }

        // PUT: api/rentals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRental(int id, Rental rental)
        {
            if (id != rental.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/rentals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/rentals/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRentalStatus(int id, [FromBody] string status)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            rental.Status = status;
            rental.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}
