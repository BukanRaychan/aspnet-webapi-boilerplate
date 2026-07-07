using ProductCatalogAPI.Data.Seeders;

namespace ProductCatalogAPI.Data;

public class DataSeeder
{
    private readonly IEnumerable<ISeeder> _seeders;
    private readonly AppDbContext _context;

    public DataSeeder(IEnumerable<ISeeder> seeders, AppDbContext context)
    {
        _seeders = seeders;
        _context = context;
    }

    public async Task SeedAsync()
    {
        foreach (var seeder in _seeders)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await seeder.SeedAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();  
                Console.WriteLine($"Error seeding data with {seeder.GetType().Name}: {ex.Message}");
            }

        }
    }
}