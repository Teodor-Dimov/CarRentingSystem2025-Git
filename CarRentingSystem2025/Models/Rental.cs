using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem2025.Models
{
    public class Rental
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [FutureDate]
        public DateTime PickupDate { get; set; }

        [Required]
        [FutureDate]
        [RentalDateRange]
        public DateTime DropoffDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Active, Completed, Cancelled, Late

        [StringLength(200)]
        public string? PickupLocation { get; set; }

        [StringLength(200)]
        public string? DropoffLocation { get; set; }

        public DateTime? ActualPickupTime { get; set; }

        public DateTime? ActualDropoffTime { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DepositAmount { get; set; }

        [StringLength(20)]
        public string? InsuranceType { get; set; } // Basic, Premium, Full Coverage

        [Range(0, double.MaxValue)]
        public decimal? InsuranceCost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LateFees { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DamageFees { get; set; }

        [StringLength(500)]
        public string? DamageDescription { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        [Range(0, 5)]
        public decimal? CustomerRating { get; set; }

        [StringLength(500)]
        public string? CustomerReview { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Car? Car { get; set; }
        public virtual Customer? Customer { get; set; }
    }
} 