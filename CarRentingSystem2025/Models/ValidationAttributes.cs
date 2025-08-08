using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem2025.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date <= DateTime.Now)
                {
                    return new ValidationResult("Date must be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class RentalDateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var rental = (Rental)validationContext.ObjectInstance;
            
            if (rental.PickupDate >= rental.DropoffDate)
            {
                return new ValidationResult("Dropoff date must be after pickup date.");
            }

            if (rental.PickupDate < DateTime.Now.Date)
            {
                return new ValidationResult("Pickup date cannot be in the past.");
            }

            // Maximum rental duration of 30 days
            if ((rental.DropoffDate - rental.PickupDate).Days > 30)
            {
                return new ValidationResult("Rental duration cannot exceed 30 days.");
            }

            return ValidationResult.Success;
        }
    }

    public class ValidLicensePlateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string licensePlate)
            {
                // Basic license plate validation (alphanumeric, 3-8 characters)
                if (!System.Text.RegularExpressions.Regex.IsMatch(licensePlate, @"^[A-Z0-9]{3,8}$"))
                {
                    return new ValidationResult("License plate must be 3-8 alphanumeric characters.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class ValidPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string phoneNumber)
            {
                // Basic phone number validation
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[\+]?[1-9][\d]{0,15}$"))
                {
                    return new ValidationResult("Please enter a valid phone number.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    if (addr.Address != email)
                    {
                        return new ValidationResult("Please enter a valid email address.");
                    }
                }
                catch
                {
                    return new ValidationResult("Please enter a valid email address.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var age = DateTime.Now.Year - dateOfBirth.Year;
                if (dateOfBirth > DateTime.Now.AddYears(-age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"You must be at least {_minimumAge} years old.");
                }
            }
            return ValidationResult.Success;
        }
    }
}

