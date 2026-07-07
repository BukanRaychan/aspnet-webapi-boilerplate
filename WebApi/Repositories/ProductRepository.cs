using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs.Common;
using WebApi.Models;

namespace WebApi.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(PaginationQueryDto pagination)
    {
        var query = _context.Products
            .Include(p => p.UnitProducts)
            .OrderBy(p => p.Id);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> GetStockCountAsync(int productId)
    {
        return await _context.UnitProducts
            .CountAsync(u => u.ProductId == productId);
    }
}