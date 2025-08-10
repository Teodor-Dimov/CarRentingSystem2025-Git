using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRentingSystem2025.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Brands
            if (!context.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand
                    {
                        Name = "BMW",
                        Country = "Germany",
                        FoundedDate = new DateTime(1916, 3, 7),
                        Headquarters = "Munich, Germany",
                        Website = "https://www.bmw.com",
                        Description = "Luxury German automaker known for sporty driving dynamics",
                        LogoUrl = "/images/brands/bmw-logo.png",
                        IsFeatured = true,
                        AverageRating = 4.5m
                    },
                    new Brand
                    {
                        Name = "Mercedes-Benz",
                        Country = "Germany",
                        FoundedDate = new DateTime(1926, 6, 28),
                        Headquarters = "Stuttgart, Germany",
                        Website = "https://www.mercedes-benz.com",
                        Description = "Premium German luxury vehicle manufacturer",
                        LogoUrl = "/images/brands/mercedes-logo.png",
                        IsFeatured = true,
                        AverageRating = 4.6m
                    },
                    new Brand
                    {
                        Name = "Volkswagen",
                        Country = "Germany",
                        FoundedDate = new DateTime(1937, 5, 28),
                        Headquarters = "Wolfsburg, Germany",
                        Website = "https://www.volkswagen.com",
                        Description = "German multinational automotive manufacturer",
                        LogoUrl = "/images/brands/vw-logo.png",
                        IsFeatured = true,
                        AverageRating = 4.3m
                    },
                    new Brand
                    {
                        Name = "Audi",
                        Country = "Germany",
                        FoundedDate = new DateTime(1909, 7, 16),
                        Headquarters = "Ingolstadt, Germany",
                        Website = "https://www.audi.com",
                        Description = "German luxury vehicle manufacturer",
                        LogoUrl = "/images/brands/audi-logo.png",
                        IsFeatured = true,
                        AverageRating = 4.4m
                    },
                    new Brand
                    {
                        Name = "Volvo",
                        Country = "Sweden",
                        FoundedDate = new DateTime(1927, 4, 14),
                        Headquarters = "Gothenburg, Sweden",
                        Website = "https://www.volvocars.com",
                        Description = "Swedish luxury vehicle manufacturer known for safety",
                        LogoUrl = "/images/brands/volvo-logo.png",
                        IsFeatured = false,
                        AverageRating = 4.2m
                    },
                    new Brand
                    {
                        Name = "Peugeot",
                        Country = "France",
                        FoundedDate = new DateTime(1810, 9, 26),
                        Headquarters = "Paris, France",
                        Website = "https://www.peugeot.com",
                        Description = "French automotive manufacturer",
                        LogoUrl = "/images/brands/peugeot-logo.png",
                        IsFeatured = false,
                        AverageRating = 4.0m
                    },
                    new Brand
                    {
                        Name = "Renault",
                        Country = "France",
                        FoundedDate = new DateTime(1899, 2, 25),
                        Headquarters = "Boulogne-Billancourt, France",
                        Website = "https://www.renault.com",
                        Description = "French multinational automobile manufacturer",
                        LogoUrl = "/images/brands/renault-logo.png",
                        IsFeatured = false,
                        AverageRating = 4.1m
                    },
                    new Brand
                    {
                        Name = "Fiat",
                        Country = "Italy",
                        FoundedDate = new DateTime(1899, 7, 11),
                        Headquarters = "Turin, Italy",
                        Website = "https://www.fiat.com",
                        Description = "Italian automobile manufacturer",
                        LogoUrl = "/images/brands/fiat-logo.png",
                        IsFeatured = false,
                        AverageRating = 3.9m
                    }
                };

                context.Brands.AddRange(brands);
                await context.SaveChangesAsync();
            }

            // Seed Cars
            if (!context.Cars.Any())
            {
                var brands = await context.Brands.ToListAsync();
                var cars = new List<Car>
                {
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "BMW").Id,
                        Brand = "BMW",
                        Model = "3 Series",
                        Year = 2023,
                        LicensePlate = "BMW-001",
                        DailyRate = 85.00m,
                        Color = "Alpine White",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 12000,
                        EngineSize = "2.0L Turbo",
                        BodyType = "Sedan",
                        DriveType = "RWD",
                        ReleaseDate = new DateTime(2023, 1, 15),
                        LastServiceDate = DateTime.Now.AddDays(-30),
                        NextServiceDate = DateTime.Now.AddDays(60),
                        Features = "BMW Live Cockpit Professional, Harman Kardon Sound, Driving Assistant Professional, Parking Assistant Plus",
                        Description = "Premium compact luxury sedan with excellent driving dynamics",
                        IsAvailable = true,
                        IsFeatured = true,
                        Rating = 4.7m,
                        NumberOfRentals = 12
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Mercedes-Benz").Id,
                        Brand = "Mercedes-Benz",
                        Model = "A-Class",
                        Year = 2023,
                        LicensePlate = "MB-002",
                        DailyRate = 75.00m,
                        Color = "Obsidian Black",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 8500,
                        EngineSize = "1.3L Turbo",
                        BodyType = "Hatchback",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2023, 1, 8),
                        LastServiceDate = DateTime.Now.AddDays(-20),
                        NextServiceDate = DateTime.Now.AddDays(70),
                        Features = "MBUX Infotainment, Burmester Sound, Ambient Lighting, Driver Assistance Package, Distronic Plus",
                        Description = "Elegant luxury sedan with advanced technology and sophisticated design",
                        IsAvailable = true,
                        IsFeatured = true,
                        Rating = 4.8m,
                        NumberOfRentals = 7
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Volkswagen").Id,
                        Brand = "Volkswagen",
                        Model = "Golf",
                        Year = 2023,
                        LicensePlate = "VW-003",
                        DailyRate = 55.00m,
                        Color = "Pure White",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 9500,
                        EngineSize = "1.4L Turbo",
                        BodyType = "Hatchback",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2023, 1, 25),
                        LastServiceDate = DateTime.Now.AddDays(-25),
                        NextServiceDate = DateTime.Now.AddDays(65),
                        Features = "VW Car-Net, Fender Premium Audio, Driver Assistance Package, Adaptive Cruise Control, Park Assist",
                        Description = "Compact hatchback with great handling and practical design",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.4m,
                        NumberOfRentals = 9
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Audi").Id,
                        Brand = "Audi",
                        Model = "A4",
                        Year = 2023,
                        LicensePlate = "AUDI-004",
                        DailyRate = 65.00m,
                        Color = "Glacier White",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 11000,
                        EngineSize = "2.0L Turbo",
                        BodyType = "Sedan",
                        DriveType = "AWD",
                        ReleaseDate = new DateTime(2023, 2, 15),
                        LastServiceDate = DateTime.Now.AddDays(-18),
                        NextServiceDate = DateTime.Now.AddDays(72),
                        Features = "Audi Virtual Cockpit, MMI Navigation Plus, Bang & Olufsen Sound, Audi Pre Sense, Adaptive Suspension",
                        Description = "Premium compact luxury sedan with quattro all-wheel drive and sophisticated technology",
                        IsAvailable = true,
                        IsFeatured = true,
                        Rating = 4.6m,
                        NumberOfRentals = 11
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Volvo").Id,
                        Brand = "Volvo",
                        Model = "XC40",
                        Year = 2023,
                        LicensePlate = "VOLVO-005",
                        DailyRate = 70.00m,
                        Color = "Crystal White",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 7800,
                        EngineSize = "2.0L Turbo",
                        BodyType = "SUV",
                        DriveType = "AWD",
                        ReleaseDate = new DateTime(2023, 3, 10),
                        LastServiceDate = DateTime.Now.AddDays(-15),
                        NextServiceDate = DateTime.Now.AddDays(75),
                        Features = "Sensus Connect, Harman Kardon Sound, Pilot Assist, City Safety, 360° Camera",
                        Description = "Compact luxury SUV with advanced safety features and Scandinavian design",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.5m,
                        NumberOfRentals = 8
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Peugeot").Id,
                        Brand = "Peugeot",
                        Model = "208",
                        Year = 2023,
                        LicensePlate = "PEUGEOT-006",
                        DailyRate = 45.00m,
                        Color = "Fusion Orange",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 6500,
                        EngineSize = "1.2L Turbo",
                        BodyType = "Hatchback",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2023, 2, 20),
                        LastServiceDate = DateTime.Now.AddDays(-12),
                        NextServiceDate = DateTime.Now.AddDays(78),
                        Features = "Peugeot i-Cockpit, Focal Sound, Advanced Grip Control, Driver Attention Alert, Lane Keeping Assist",
                        Description = "Stylish compact hatchback with innovative interior design",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.2m,
                        NumberOfRentals = 5
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Renault").Id,
                        Brand = "Renault",
                        Model = "Clio",
                        Year = 2023,
                        LicensePlate = "RENAULT-007",
                        DailyRate = 40.00m,
                        Color = "Arctic White",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 7200,
                        EngineSize = "1.0L Turbo",
                        BodyType = "Hatchback",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2023, 1, 30),
                        LastServiceDate = DateTime.Now.AddDays(-8),
                        NextServiceDate = DateTime.Now.AddDays(82),
                        Features = "Renault EASY LINK, Bose Sound, Multi-Sense, Advanced Emergency Braking, Lane Departure Warning",
                        Description = "Compact city car with excellent fuel efficiency and modern technology",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.0m,
                        NumberOfRentals = 4
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Fiat").Id,
                        Brand = "Fiat",
                        Model = "500",
                        Year = 2023,
                        LicensePlate = "FIAT-008",
                        DailyRate = 35.00m,
                        Color = "Bianco",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 4,
                        Mileage = 5800,
                        EngineSize = "1.0L Turbo",
                        BodyType = "Hatchback",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2023, 2, 5),
                        LastServiceDate = DateTime.Now.AddDays(-5),
                        NextServiceDate = DateTime.Now.AddDays(85),
                        Features = "Uconnect LIVE, Beats Audio, Advanced Driver Assistance Systems, 7\" HD Touchscreen",
                        Description = "Iconic Italian city car with retro styling and modern features",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.1m,
                        NumberOfRentals = 6
                    }
                };

                context.Cars.AddRange(cars);
                await context.SaveChangesAsync();
            }

            // Seed Sample Customers
            if (!context.Customers.Any())
            {
                var customers = new List<Customer>
                {
                    new Customer
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@email.com",
                        PhoneNumber = "+1-555-0101",
                        Address = "123 Main Street",
                        City = "New York",
                        PostalCode = "10001",
                        Country = "USA",
                        DriverLicenseNumber = "DL123456789",
                        DriverLicenseExpiry = DateTime.Now.AddYears(5),
                        DateOfBirth = new DateTime(1990, 5, 15),
                        EmergencyContactName = "Sarah Doe",
                        EmergencyContactPhone = "+1-555-0102",
                        InsuranceProvider = "State Farm",
                        InsurancePolicyNumber = "SF123456789",
                        InsuranceExpiryDate = DateTime.Now.AddYears(2),
                        MembershipLevel = "Gold",
                        MembershipStartDate = DateTime.Now.AddMonths(-6),
                        MembershipExpiryDate = DateTime.Now.AddYears(1),
                        TotalSpent = 1250.00m,
                        TotalRentals = 8,
                        AverageRating = 4.5m,
                        IsVerified = true,
                        Notes = "Preferred customer, always returns cars on time"
                    },
                    new Customer
                    {
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane.smith@email.com",
                        PhoneNumber = "+1-555-0103",
                        Address = "456 Oak Avenue",
                        City = "Los Angeles",
                        PostalCode = "90210",
                        Country = "USA",
                        DriverLicenseNumber = "DL987654321",
                        DriverLicenseExpiry = DateTime.Now.AddYears(4),
                        DateOfBirth = new DateTime(1988, 8, 22),
                        EmergencyContactName = "Michael Smith",
                        EmergencyContactPhone = "+1-555-0104",
                        InsuranceProvider = "Allstate",
                        InsurancePolicyNumber = "AL987654321",
                        InsuranceExpiryDate = DateTime.Now.AddYears(1),
                        MembershipLevel = "Silver",
                        MembershipStartDate = DateTime.Now.AddMonths(-3),
                        MembershipExpiryDate = DateTime.Now.AddMonths(9),
                        TotalSpent = 850.00m,
                        TotalRentals = 5,
                        AverageRating = 4.3m,
                        IsVerified = true,
                        Notes = "Business traveler, prefers luxury vehicles"
                    },
                    new Customer
                    {
                        FirstName = "Mike",
                        LastName = "Johnson",
                        Email = "mike.johnson@email.com",
                        PhoneNumber = "+1-555-0105",
                        Address = "789 Pine Street",
                        City = "Chicago",
                        PostalCode = "60601",
                        Country = "USA",
                        DriverLicenseNumber = "DL456789123",
                        DriverLicenseExpiry = DateTime.Now.AddYears(3),
                        DateOfBirth = new DateTime(1992, 3, 10),
                        EmergencyContactName = "Lisa Johnson",
                        EmergencyContactPhone = "+1-555-0106",
                        InsuranceProvider = "Progressive",
                        InsurancePolicyNumber = "PR456789123",
                        InsuranceExpiryDate = DateTime.Now.AddMonths(6),
                        MembershipLevel = "Standard",
                        MembershipStartDate = DateTime.Now.AddMonths(-1),
                        MembershipExpiryDate = DateTime.Now.AddMonths(11),
                        TotalSpent = 320.00m,
                        TotalRentals = 2,
                        AverageRating = 4.0m,
                        IsVerified = false,
                        Notes = "New customer, first rental was successful"
                    }
                };

                context.Customers.AddRange(customers);
                await context.SaveChangesAsync();
            }

            // Seed Sample Rentals
            if (!context.Rentals.Any())
            {
                var cars = context.Cars.ToList();
                var customers = context.Customers.ToList();

                if (cars.Any() && customers.Any())
                {
                    var rentals = new List<Rental>
                    {
                        new Rental
                        {
                            CarId = cars[0].Id,
                            CustomerId = customers[0].Id,
                            PickupDate = DateTime.Now.AddDays(1),
                            DropoffDate = DateTime.Now.AddDays(3),
                            TotalAmount = 255.00m,
                            Status = "Active",
                            PickupLocation = "Airport Terminal 1",
                            DropoffLocation = "Downtown Office",
                            ActualPickupTime = DateTime.Now.AddDays(1).AddHours(10),
                            ActualDropoffTime = DateTime.Now.AddDays(3).AddHours(16),
                            DepositAmount = 100.00m,
                            InsuranceType = "Basic",
                            InsuranceCost = 25.00m,
                            LateFees = 0.00m,
                            DamageFees = 0.00m,
                            SpecialRequests = "GPS navigation preferred",
                            CustomerRating = 5,
                            CustomerReview = "Excellent service, car was in perfect condition",
                            CreatedAt = DateTime.Now.AddDays(-2)
                        },
                        new Rental
                        {
                            CarId = cars[1].Id,
                            CustomerId = customers[1].Id,
                            PickupDate = DateTime.Now.AddDays(2),
                            DropoffDate = DateTime.Now.AddDays(5),
                            TotalAmount = 375.00m,
                            Status = "Pending",
                            PickupLocation = "Hotel Lobby",
                            DropoffLocation = "Airport Terminal 2",
                            DepositAmount = 150.00m,
                            InsuranceType = "Premium",
                            InsuranceCost = 50.00m,
                            LateFees = 0.00m,
                            DamageFees = 0.00m,
                            SpecialRequests = "Child seat needed",
                            CreatedAt = DateTime.Now.AddDays(-1)
                        },
                        new Rental
                        {
                            CarId = cars[2].Id,
                            CustomerId = customers[2].Id,
                            PickupDate = DateTime.Now.AddDays(-5),
                            DropoffDate = DateTime.Now.AddDays(-2),
                            TotalAmount = 165.00m,
                            Status = "Completed",
                            PickupLocation = "Downtown Office",
                            DropoffLocation = "Airport Terminal 1",
                            ActualPickupTime = DateTime.Now.AddDays(-5).AddHours(9),
                            ActualDropoffTime = DateTime.Now.AddDays(-2).AddHours(17),
                            DepositAmount = 75.00m,
                            InsuranceType = "Standard",
                            InsuranceCost = 30.00m,
                            LateFees = 0.00m,
                            DamageFees = 0.00m,
                            CustomerRating = 4,
                            CustomerReview = "Good experience, would rent again",
                            CreatedAt = DateTime.Now.AddDays(-7)
                        }
                    };

                    context.Rentals.AddRange(rentals);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 