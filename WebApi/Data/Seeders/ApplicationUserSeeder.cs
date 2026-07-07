using Microsoft.AspNetCore.Identity;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Data.Seeders;

public class ApplicationUserSeeder : ISeeder
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserSeeder(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        if (_userManager.Users.Any()) return;

        var users = new List<(ApplicationUser user, string password)>
        {
            (new ApplicationUser
            {
                FirstName = "Admin",
                LastName = "User",
                UserName = "admin",
                Email = "admin@example.com",
                CreatedAt = DateTime.UtcNow
            }, "password"),

            (new ApplicationUser
            {
                FirstName = "Regular",
                LastName = "User",
                UserName = "user",
                Email = "user@example.com",
                CreatedAt = DateTime.UtcNow
            }, "password")
        };

        foreach (var (user, password) in users)
        {
            await _userManager.CreateAsync(user, password);
        }
    }
}