using ProductCatalogAPI.DTOs.AuthDtos;
using ProductCatalogAPI.DTOs.ProductDtos;

namespace ProductCatalogAPI.DTOs.UnitProductDtos;

public class UnitProductResponseDto
{
    public int Id { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProductResponseDto? Product { get; set; }
    public UserInfoResponseDto? User { get; set; }
}