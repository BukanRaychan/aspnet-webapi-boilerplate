namespace ProductCatalogAPI.Models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ICollection<UnitProduct> UnitProducts { get; set; } = new List<UnitProduct>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}