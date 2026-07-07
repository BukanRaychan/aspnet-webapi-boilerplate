using WebApi.DTOs.AuthDtos;
using WebApi.DTOs.ProductDtos;

namespace WebApi.DTOs.UnitProductDtos;

public class UnitProductResponseDto
{
    public int Id { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProductResponseDto? Product { get; set; }
    public UserInfoResponseDto? User { get; set; }
}