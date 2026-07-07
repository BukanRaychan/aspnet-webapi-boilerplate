using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Repositories;

public interface IUnitProductRepository
{
    Task<List<UnitProduct>> GetAllAsync();
    Task<UnitProduct?> GetByIdAsync(int id);
    Task<List<UnitProduct>> GetByProductIdAsync(int productId);
    Task<UnitProduct> CreateAsync(UnitProduct unitProduct);
    Task<UnitProduct?> UpdateAsync(UnitProduct unitProduct);
    Task<bool> DeleteAsync(int id);
}