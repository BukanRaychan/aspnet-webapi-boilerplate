using WebApi.DTOs.UnitProductDtos;

namespace WebApi.Services;

public interface IUnitProductService
{
    Task<List<UnitProductResponseDto>> GetAllAsync();
    Task<UnitProductResponseDto?> GetByIdAsync(int id);
    Task<List<UnitProductResponseDto>> GetByProductIdAsync(int productId);
    Task<UnitProductResponseDto> CreateAsync(string userId, CreateUnitProductDto dto);
    Task<UnitProductResponseDto?> UpdateAsync(int id, UpdateUnitProductDto dto);
    Task<bool> DeleteAsync(int id);
}