using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarRentingSystem2025.Services
{
    public interface IValidationService
    {
        bool ValidateRental(Rental rental, ModelStateDictionary modelState);
        bool ValidateCar(Car car, ModelStateDictionary modelState);
        bool ValidateCustomer(Customer customer, ModelStateDictionary modelState);
        bool ValidateBrand(Brand brand, ModelStateDictionary modelState);
    }

    public class ValidationService : IValidationService
    {
        public bool ValidateRental(Rental rental, ModelStateDictionary modelState)
        {
            bool isValid = true;

            // Check if car is available
            if (rental.CarId > 0)
            {
                // This would typically check against the database
                // For now, we'll assume the car exists and is available
            }

            // Validate dates
            if (rental.PickupDate >= rental.DropoffDate)
            {
                modelState.AddModelError("DropoffDate", "Dropoff date must be after pickup date.");
                isValid = false;
            }

            if (rental.PickupDate < DateTime.Now.Date)
            {
                modelState.AddModelError("PickupDate", "Pickup date cannot be in the past.");
                isValid = false;
            }

            if ((rental.DropoffDate - rental.PickupDate).Days > 30)
            {
                modelState.AddModelError("DropoffDate", "Rental duration cannot exceed 30 days.");
                isValid = false;
            }

            // Validate total amount
            if (rental.TotalAmount <= 0)
            {
                modelState.AddModelError("TotalAmount", "Total amount must be greater than zero.");
                isValid = false;
            }

            return isValid;
        }

        public bool ValidateCar(Car car, ModelStateDictionary modelState)
        {
            bool isValid = true;

            // Validate license plate format
            if (!string.IsNullOrEmpty(car.LicensePlate))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(car.LicensePlate, @"^[A-Z0-9]{3,8}$"))
                {
                    modelState.AddModelError("LicensePlate", "License plate must be 3-8 alphanumeric characters.");
                    isValid = false;
                }
            }

            // Validate year
            if (car.Year < 1900 || car.Year > DateTime.Now.Year + 1)
            {
                modelState.AddModelError("Year", "Year must be between 1900 and next year.");
                isValid = false;
            }

            // Validate daily rate
            if (car.DailyRate <= 0)
            {
                modelState.AddModelError("DailyRate", "Daily rate must be greater than zero.");
                isValid = false;
            }

            // Validate mileage
            if (car.Mileage < 0)
            {
                modelState.AddModelError("Mileage", "Mileage cannot be negative.");
                isValid = false;
            }

            return isValid;
        }

        public bool ValidateCustomer(Customer customer, ModelStateDictionary modelState)
        {
            bool isValid = true;

            // Validate email format
            if (!string.IsNullOrEmpty(customer.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(customer.Email);
                    if (addr.Address != customer.Email)
                    {
                        modelState.AddModelError("Email", "Please enter a valid email address.");
                        isValid = false;
                    }
                }
                catch
                {
                    modelState.AddModelError("Email", "Please enter a valid email address.");
                    isValid = false;
                }
            }

            // Validate phone number
            if (!string.IsNullOrEmpty(customer.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(customer.PhoneNumber, @"^[\+]?[1-9][\d]{0,15}$"))
                {
                    modelState.AddModelError("PhoneNumber", "Please enter a valid phone number.");
                    isValid = false;
                }
            }

            // Validate age
            if (customer.DateOfBirth != default)
            {
                var age = DateTime.Now.Year - customer.DateOfBirth.Year;
                if (customer.DateOfBirth > DateTime.Now.AddYears(-age))
                {
                    age--;
                }

                if (age < 18)
                {
                    modelState.AddModelError("DateOfBirth", "You must be at least 18 years old.");
                    isValid = false;
                }
            }

            return isValid;
        }

        public bool ValidateBrand(Brand brand, ModelStateDictionary modelState)
        {
            bool isValid = true;

            // Validate name
            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                modelState.AddModelError("Name", "Brand name is required.");
                isValid = false;
            }

            // Validate email if provided
            if (!string.IsNullOrEmpty(brand.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(brand.Email);
                    if (addr.Address != brand.Email)
                    {
                        modelState.AddModelError("Email", "Please enter a valid email address.");
                        isValid = false;
                    }
                }
                catch
                {
                    modelState.AddModelError("Email", "Please enter a valid email address.");
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}


