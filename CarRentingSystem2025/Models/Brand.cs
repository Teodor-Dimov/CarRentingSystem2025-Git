using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem2025.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? LogoUrl { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(100)]
        public string? Headquarters { get; set; }

        public DateTime? FoundedDate { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Slogan { get; set; }

        [StringLength(100)]
        public string? CEO { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } = "Active"; // Active, Inactive, Discontinued

        [Range(0, 5)]
        public decimal AverageRating { get; set; } = 0;

        public int TotalCars { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    }
} 