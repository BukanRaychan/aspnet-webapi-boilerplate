using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Data.Seeders;

public class ProductSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public ProductSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (_context.Products.Any()) return;

        var products = new List<Product>
        {
            new Product
            {
                Name = "Laptop",
                Description = "High performance gaming laptop",
                Price = 15000000,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Mouse",
                Description = "Wireless ergonomic mouse",
                Price = 250000,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Keyboard",
                Description = "Mechanical RGB keyboard",
                Price = 800000,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
    }
}