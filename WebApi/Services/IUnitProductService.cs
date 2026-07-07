using WebApi.DTOs.Common;
using WebApi.DTOs.UnitProductDtos;

namespace WebApi.Services;

public interface IUnitProductService
{
    Task<PagedResponse<UnitProductResponseDto>> GetPagedAsync(PaginationQueryDto pagination);
    Task<UnitProductResponseDto?> GetByIdAsync(int id);
    Task<List<UnitProductResponseDto>> GetByProductIdAsync(int productId);
    Task<UnitProductResponseDto> CreateAsync(string userId, CreateUnitProductDto dto);
    Task<UnitProductResponseDto?> UpdateAsync(int id, UpdateUnitProductDto dto);
    Task<bool> DeleteAsync(int id);
}