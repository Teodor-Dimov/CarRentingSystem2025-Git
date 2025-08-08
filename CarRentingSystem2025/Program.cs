using CarRentingSystem2025.Data;
using CarRentingSystem2025.Models;
using CarRentingSystem2025.Middleware;
using CarRentingSystem2025.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, 
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false; // Allow immediate sign in
    options.SignIn.RequireConfirmedEmail = false; // Don't require email confirmation
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false; // Make it less strict
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure authentication options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddControllersWithViews();

// Register validation service
builder.Services.AddScoped<IValidationService, ValidationService>();

// Register performance and cache services
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        string[] roleNames = { "Administrator", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create admin user
        var adminEmail = "admin@carrent.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, "Admin123!"); // Change password after first login
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }

        // Create test users
        var testUsers = new[]
        {
            new { Email = "john.doe@email.com", Password = "Test123!", FirstName = "John", LastName = "Doe" },
            new { Email = "jane.smith@email.com", Password = "Test123!", FirstName = "Jane", LastName = "Smith" },
            new { Email = "mike.johnson@email.com", Password = "Test123!", FirstName = "Mike", LastName = "Johnson" }
        };

        foreach (var testUser in testUsers)
        {
            var user = await userManager.FindByEmailAsync(testUser.Email);
            if (user == null)
            {
                var newUser = new IdentityUser 
                { 
                    UserName = testUser.Email, 
                    Email = testUser.Email, 
                    EmailConfirmed = true 
                };
                var result = await userManager.CreateAsync(newUser, testUser.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                    
                    // Create corresponding customer profile
                    var customer = new Customer
                    {
                        UserId = newUser.Id,
                        FirstName = testUser.FirstName,
                        LastName = testUser.LastName,
                        Email = testUser.Email,
                        PhoneNumber = "+1-555-0101",
                        DateOfBirth = DateTime.Now.AddYears(-30),
                        Address = "123 Test Street",
                        City = "Test City",
                        PostalCode = "12345",
                        Country = "USA",
                        DriverLicenseNumber = "DL123456789",
                        DriverLicenseExpiry = DateTime.Now.AddYears(5),
                        MembershipLevel = "Standard",
                        MembershipStartDate = DateTime.Now,
                        MembershipExpiryDate = DateTime.Now.AddYears(1),
                        IsVerified = true,
                        CreatedAt = DateTime.Now
                    };
                    context.Customers.Add(customer);
                }
            }
        }

        // Initialize database with sample data
        await DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        // Log the error but don't crash the application
        Console.WriteLine($"Database initialization error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add custom error handling middleware
app.UseCustomErrorHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
