using WebApi.DTOs.Common;
using WebApi.Models;

namespace WebApi.Repositories;

public interface IUnitProductRepository
{
    Task<(List<UnitProduct> Items, int TotalCount)> GetPagedAsync(PaginationQueryDto pagination);
    Task<UnitProduct?> GetByIdAsync(int id);
    Task<List<UnitProduct>> GetByProductIdAsync(int productId);
    Task<UnitProduct> CreateAsync(UnitProduct unitProduct);
    Task<UnitProduct?> UpdateAsync(UnitProduct unitProduct);
    Task<bool> DeleteAsync(int id);
}