using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem2025.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int RentalId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // Credit Card, Debit Card, Cash, Bank Transfer, PayPal

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded, Cancelled

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(100)]
        public string? CardLastFourDigits { get; set; }

        [StringLength(50)]
        public string? CardType { get; set; } // Visa, MasterCard, American Express, etc.

        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(50)]
        public string? AccountNumber { get; set; }

        [StringLength(20)]
        public string? PaymentType { get; set; } // Deposit, Full Payment, Partial Payment, Refund

        [Range(0, double.MaxValue)]
        public decimal? RefundAmount { get; set; }

        public DateTime? RefundDate { get; set; }

        [StringLength(100)]
        public string? RefundReason { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? FailureReason { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Rental Rental { get; set; } = null!;
    }
} 