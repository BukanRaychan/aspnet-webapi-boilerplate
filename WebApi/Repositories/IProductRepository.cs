using WebApi.DTOs.Common;
using WebApi.Models;

namespace WebApi.Repositories;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(PaginationQueryDto pagination);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<int> GetStockCountAsync(int productId);
}