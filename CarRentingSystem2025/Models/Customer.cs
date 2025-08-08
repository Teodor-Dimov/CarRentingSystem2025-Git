using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem2025.Models
{
    public class Customer
    {
        public int Id { get; set; }

        // Link to ASP.NET Identity User
        [StringLength(450)]
        public string? UserId { get; set; } // Foreign key to IdentityUser.Id

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [ValidEmailDomain]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [ValidPhoneNumber]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DriverLicenseNumber { get; set; } = string.Empty;

        public DateTime DriverLicenseExpiry { get; set; }

        [MinimumAge(18)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(100)]
        public string? InsuranceProvider { get; set; }

        [StringLength(50)]
        public string? InsurancePolicyNumber { get; set; }

        public DateTime? InsuranceExpiryDate { get; set; }

        [StringLength(20)]
        public string MembershipLevel { get; set; } = "Standard"; // Standard, Silver, Gold, Platinum

        public DateTime MembershipStartDate { get; set; } = DateTime.Now;

        public DateTime? MembershipExpiryDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalSpent { get; set; } = 0;

        public int TotalRentals { get; set; } = 0;

        [Range(0, 5)]
        public decimal AverageRating { get; set; } = 0;

        public bool IsVerified { get; set; } = false;

        public bool IsBlacklisted { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        // Computed property for display
        public string DisplayName => $"{FirstName} {LastName} - {Email}";
    }
} 