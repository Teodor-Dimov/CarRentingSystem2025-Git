using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem2025.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2030)]
        public int Year { get; set; }

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DailyRate { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string FuelType { get; set; } = string.Empty; // Gasoline, Diesel, Electric, Hybrid

        [Required]
        [StringLength(10)]
        public string Transmission { get; set; } = string.Empty; // Manual, Automatic

        [Range(1, 10)]
        public int Seats { get; set; }

        [Range(0, 1000000)]
        public int Mileage { get; set; }

        [StringLength(20)]
        public string EngineSize { get; set; } = string.Empty; // e.g., "2.0L", "3.0L V6"

        [StringLength(50)]
        public string BodyType { get; set; } = string.Empty; // Sedan, SUV, Hatchback, Coupe, etc.

        [StringLength(20)]
        public string DriveType { get; set; } = string.Empty; // FWD, RWD, AWD, 4WD

        public DateTime ReleaseDate { get; set; }

        public DateTime LastServiceDate { get; set; }

        public DateTime NextServiceDate { get; set; }

        [StringLength(500)]
        public string? Features { get; set; } // Comma-separated features like "GPS, Bluetooth, Backup Camera"

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        [Range(0, 5)]
        public decimal Rating { get; set; } = 0;

        public int NumberOfRentals { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Brand BrandEntity { get; set; } = null!;
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        // Computed property for display
        public string DisplayName => $"{Brand} {Model} ({Year}) - {LicensePlate}";
    }
}
