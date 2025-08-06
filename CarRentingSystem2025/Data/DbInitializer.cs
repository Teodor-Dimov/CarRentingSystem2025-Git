using CarRentingSystem2025.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentingSystem2025.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Ensure database is created with all tables
            await context.Database.EnsureCreatedAsync();

            // Seed Brands
            if (!context.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand 
                    { 
                        Name = "Toyota", 
                        Description = "Japanese automotive manufacturer known for reliability and fuel efficiency", 
                        Country = "Japan",
                        Headquarters = "Toyota City, Aichi, Japan",
                        FoundedDate = new DateTime(1937, 8, 28),
                        Website = "https://www.toyota.com",
                        PhoneNumber = "+81-565-28-2121",
                        Email = "info@toyota.com",
                        Slogan = "Let's Go Places",
                        CEO = "Koji Sato",
                        Status = "Active",
                        AverageRating = 4.5m,
                        IsFeatured = true
                    },
                    new Brand 
                    { 
                        Name = "Honda", 
                        Description = "Japanese multinational corporation known for motorcycles and automobiles", 
                        Country = "Japan",
                        Headquarters = "Minato, Tokyo, Japan",
                        FoundedDate = new DateTime(1948, 9, 24),
                        Website = "https://www.honda.com",
                        PhoneNumber = "+81-3-3423-1111",
                        Email = "info@honda.com",
                        Slogan = "The Power of Dreams",
                        CEO = "Toshihiro Mibe",
                        Status = "Active",
                        AverageRating = 4.3m,
                        IsFeatured = true
                    },
                    new Brand 
                    { 
                        Name = "BMW", 
                        Description = "German luxury vehicle manufacturer known for performance and innovation", 
                        Country = "Germany",
                        Headquarters = "Munich, Bavaria, Germany",
                        FoundedDate = new DateTime(1916, 3, 7),
                        Website = "https://www.bmw.com",
                        PhoneNumber = "+49-89-382-0",
                        Email = "info@bmw.com",
                        Slogan = "The Ultimate Driving Machine",
                        CEO = "Oliver Zipse",
                        Status = "Active",
                        AverageRating = 4.7m,
                        IsFeatured = true
                    },
                    new Brand 
                    { 
                        Name = "Mercedes-Benz", 
                        Description = "German automotive manufacturer known for luxury and innovation", 
                        Country = "Germany",
                        Headquarters = "Stuttgart, Baden-Württemberg, Germany",
                        FoundedDate = new DateTime(1926, 6, 28),
                        Website = "https://www.mercedes-benz.com",
                        PhoneNumber = "+49-711-17-0",
                        Email = "info@mercedes-benz.com",
                        Slogan = "The Best or Nothing",
                        CEO = "Ola Källenius",
                        Status = "Active",
                        AverageRating = 4.8m,
                        IsFeatured = true
                    },
                    new Brand 
                    { 
                        Name = "Ford", 
                        Description = "American multinational automaker with a rich history", 
                        Country = "USA",
                        Headquarters = "Dearborn, Michigan, USA",
                        FoundedDate = new DateTime(1903, 6, 16),
                        Website = "https://www.ford.com",
                        PhoneNumber = "+1-800-392-3673",
                        Email = "info@ford.com",
                        Slogan = "Built Ford Tough",
                        CEO = "Jim Farley",
                        Status = "Active",
                        AverageRating = 4.2m,
                        IsFeatured = true
                    },
                    new Brand 
                    { 
                        Name = "Chevrolet", 
                        Description = "American automobile division of General Motors", 
                        Country = "USA",
                        Headquarters = "Detroit, Michigan, USA",
                        FoundedDate = new DateTime(1911, 11, 3),
                        Website = "https://www.chevrolet.com",
                        PhoneNumber = "+1-800-222-1020",
                        Email = "info@chevrolet.com",
                        Slogan = "Find New Roads",
                        CEO = "Mary Barra",
                        Status = "Active",
                        AverageRating = 4.1m,
                        IsFeatured = false
                    },
                    new Brand 
                    { 
                        Name = "Volkswagen", 
                        Description = "German multinational automotive manufacturer", 
                        Country = "Germany",
                        Headquarters = "Wolfsburg, Lower Saxony, Germany",
                        FoundedDate = new DateTime(1937, 5, 28),
                        Website = "https://www.volkswagen.com",
                        PhoneNumber = "+49-5361-9-0",
                        Email = "info@volkswagen.com",
                        Slogan = "Das Auto",
                        CEO = "Oliver Blume",
                        Status = "Active",
                        AverageRating = 4.4m,
                        IsFeatured = false
                    },
                    new Brand 
                    { 
                        Name = "Audi", 
                        Description = "German automobile manufacturer known for luxury and technology", 
                        Country = "Germany",
                        Headquarters = "Ingolstadt, Bavaria, Germany",
                        FoundedDate = new DateTime(1909, 7, 16),
                        Website = "https://www.audi.com",
                        PhoneNumber = "+49-841-89-0",
                        Email = "info@audi.com",
                        Slogan = "Vorsprung durch Technik",
                        CEO = "Gernot Döllner",
                        Status = "Active",
                        AverageRating = 4.6m,
                        IsFeatured = true
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
                        BrandId = brands.First(b => b.Name == "Toyota").Id,
                        Brand = "Toyota",
                        Model = "Camry",
                        Year = 2023,
                        LicensePlate = "ABC-123",
                        DailyRate = 75.00m,
                        Color = "White",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 15000,
                        EngineSize = "2.5L",
                        BodyType = "Sedan",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2023, 1, 15),
                        LastServiceDate = DateTime.Now.AddDays(-30),
                        NextServiceDate = DateTime.Now.AddDays(60),
                        Features = "GPS Navigation, Bluetooth, Backup Camera, Apple CarPlay, Android Auto, Lane Departure Warning, Adaptive Cruise Control",
                        Description = "Comfortable sedan with excellent fuel economy and advanced safety features",
                        IsAvailable = true,
                        IsFeatured = true,
                        Rating = 4.5m,
                        NumberOfRentals = 12
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Honda").Id,
                        Brand = "Honda",
                        Model = "Civic",
                        Year = 2022,
                        LicensePlate = "DEF-456",
                        DailyRate = 65.00m,
                        Color = "Blue",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 22000,
                        EngineSize = "2.0L",
                        BodyType = "Sedan",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2022, 3, 10),
                        LastServiceDate = DateTime.Now.AddDays(-45),
                        NextServiceDate = DateTime.Now.AddDays(45),
                        Features = "Honda Sensing, Bluetooth, Backup Camera, Apple CarPlay, Android Auto, Blind Spot Monitoring",
                        Description = "Reliable compact car perfect for city driving with excellent safety ratings",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.3m,
                        NumberOfRentals = 8
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "BMW").Id,
                        Brand = "BMW",
                        Model = "X5",
                        Year = 2023,
                        LicensePlate = "GHI-789",
                        DailyRate = 120.00m,
                        Color = "Black",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 7,
                        Mileage = 8500,
                        EngineSize = "3.0L Twin-Turbo",
                        BodyType = "SUV",
                        DriveType = "AWD",
                        ReleaseDate = new DateTime(2023, 2, 20),
                        LastServiceDate = DateTime.Now.AddDays(-15),
                        NextServiceDate = DateTime.Now.AddDays(75),
                        Features = "BMW iDrive, Navigation, Harman Kardon Sound, Panoramic Sunroof, Heated Seats, Adaptive Suspension, Parking Assistant",
                        Description = "Luxury SUV with premium features and exceptional performance",
                        IsAvailable = true,
                        IsFeatured = true,
                        Rating = 4.7m,
                        NumberOfRentals = 5
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Mercedes-Benz").Id,
                        Brand = "Mercedes-Benz",
                        Model = "C-Class",
                        Year = 2023,
                        LicensePlate = "JKL-012",
                        DailyRate = 110.00m,
                        Color = "Silver",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 12000,
                        EngineSize = "2.0L Turbo",
                        BodyType = "Sedan",
                        DriveType = "RWD",
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
                        BrandId = brands.First(b => b.Name == "Ford").Id,
                        Brand = "Ford",
                        Model = "Mustang",
                        Year = 2023,
                        LicensePlate = "MNO-345",
                        DailyRate = 95.00m,
                        Color = "Red",
                        FuelType = "Gasoline",
                        Transmission = "Manual",
                        Seats = 4,
                        Mileage = 8000,
                        EngineSize = "5.0L V8",
                        BodyType = "Coupe",
                        DriveType = "RWD",
                        ReleaseDate = new DateTime(2023, 3, 5),
                        LastServiceDate = DateTime.Now.AddDays(-10),
                        NextServiceDate = DateTime.Now.AddDays(80),
                        Features = "SYNC 4, Bang & Olufsen Sound, Track Apps, Launch Control, Line Lock, Selectable Drive Modes",
                        Description = "Iconic American muscle car with powerful V8 engine and classic styling",
                        IsAvailable = true,
                        IsFeatured = true,
                        Rating = 4.6m,
                        NumberOfRentals = 15
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Chevrolet").Id,
                        Brand = "Chevrolet",
                        Model = "Malibu",
                        Year = 2022,
                        LicensePlate = "PQR-678",
                        DailyRate = 70.00m,
                        Color = "Gray",
                        FuelType = "Gasoline",
                        Transmission = "Automatic",
                        Seats = 5,
                        Mileage = 18000,
                        EngineSize = "1.5L Turbo",
                        BodyType = "Sedan",
                        DriveType = "FWD",
                        ReleaseDate = new DateTime(2022, 4, 12),
                        LastServiceDate = DateTime.Now.AddDays(-35),
                        NextServiceDate = DateTime.Now.AddDays(55),
                        Features = "Chevrolet Infotainment 3, Teen Driver Technology, OnStar, Forward Collision Alert, Lane Keep Assist",
                        Description = "Comfortable mid-size sedan with modern technology and good fuel economy",
                        IsAvailable = true,
                        IsFeatured = false,
                        Rating = 4.1m,
                        NumberOfRentals = 6
                    },
                    new Car
                    {
                        BrandId = brands.First(b => b.Name == "Volkswagen").Id,
                        Brand = "Volkswagen",
                        Model = "Golf",
                        Year = 2023,
                        LicensePlate = "STU-901",
                        DailyRate = 60.00m,
                        Color = "White",
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
                        LicensePlate = "VWX-234",
                        DailyRate = 100.00m,
                        Color = "Black",
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
        }
    }
} 