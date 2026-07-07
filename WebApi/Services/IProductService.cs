using WebApi.DTOs.Common;
using WebApi.DTOs.ProductDtos;

namespace WebApi.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponseDto>> GetPagedAsync(PaginationQueryDto pagination);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}