using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Data.Seeders;

public class UnitProductSeeder : ISeeder
{
    private readonly AppDbContext _context;

    public UnitProductSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (_context.UnitProducts.Any()) return;

        var firstProduct = _context.Products.OrderBy(p => p.Id).FirstOrDefault();
        var firstUser = _context.Users.OrderBy(u => u.Id).FirstOrDefault();

        if (firstProduct == null || firstUser == null) return;

        var unitProducts = new List<UnitProduct>
        {
            new UnitProduct
            {
                SerialNumber = "SN-001",
                ProductId = firstProduct.Id,
                UserId = firstUser.Id,
                CreatedAt = DateTime.UtcNow
            },
            new UnitProduct
            {
                SerialNumber = "SN-002",
                ProductId = firstProduct.Id,
                UserId = firstUser.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.UnitProducts.AddRange(unitProducts);
        await _context.SaveChangesAsync();
    }
}