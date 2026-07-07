using Microsoft.AspNetCore.Identity;

namespace ProductCatalogAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<UnitProduct> UnitProducts { get; set; } = new List<UnitProduct>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
