using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using System.Text.Json;

namespace CarRentingSystem2025.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _context.Cars
                .Include(c => c.BrandEntity)
                .ToListAsync();
            
            return Ok(cars);
        }

        // GET: api/cars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.BrandEntity)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        // GET: api/cars/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Car>>> GetAvailableCars()
        {
            var availableCars = await _context.Cars
                .Include(c => c.BrandEntity)
                .Where(c => c.IsAvailable)
                .ToListAsync();
            
            return Ok(availableCars);
        }

        // GET: api/cars/brand/{brandId}
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarsByBrand(int brandId)
        {
            var cars = await _context.Cars
                .Include(c => c.BrandEntity)
                .Where(c => c.BrandId == brandId)
                .ToListAsync();
            
            return Ok(cars);
        }

        // POST: api/cars
        [HttpPost]
        public async Task<ActionResult<Car>> CreateCar(Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }

        // PUT: api/cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
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

        // DELETE: api/cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}




