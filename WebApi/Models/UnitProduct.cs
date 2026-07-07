namespace ProductCatalogAPI.Models;

public class UnitProduct
{
    public int Id { get; set; }
    public string? SerialNumber { get; set; }
    public int ProductId { get; set; } = 0;
    public Product? Product { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}