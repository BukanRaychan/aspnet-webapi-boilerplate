using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs.Common;
using WebApi.Models;

namespace WebApi.Repositories;

public class UnitProductRepository : IUnitProductRepository
{
    private readonly AppDbContext _context;

    public UnitProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<UnitProduct> Items, int TotalCount)> GetPagedAsync(PaginationQueryDto pagination)
    {
        var query = _context.UnitProducts
            .Include(u => u.Product)
            .Include(u => u.User)
            .OrderBy(u => u.Id);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<UnitProduct?> GetByIdAsync(int id)
    {
        return await _context.UnitProducts
            .Include(u => u.Product)
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<UnitProduct>> GetByProductIdAsync(int productId)
    {
        return await _context.UnitProducts
            .Include(u => u.Product)
            .Include(u => u.User)
            .Where(u => u.ProductId == productId)
            .ToListAsync();
    }

    public async Task<UnitProduct> CreateAsync(UnitProduct unitProduct)
    {
        _context.UnitProducts.Add(unitProduct);
        await _context.SaveChangesAsync();
        await LoadReference(unitProduct);
        return unitProduct;
    }

    public async Task<UnitProduct?> UpdateAsync(UnitProduct unitProduct)
    {
        _context.UnitProducts.Update(unitProduct);
        await _context.SaveChangesAsync();
        await LoadReference(unitProduct);
        return unitProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var unitProduct = await _context.UnitProducts.FirstOrDefaultAsync(u => u.Id == id);
        if (unitProduct == null) return false;

        _context.UnitProducts.Remove(unitProduct);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task LoadReference(UnitProduct unitProduct)
    {
        await _context.Entry(unitProduct).Reference(u => u.Product).LoadAsync();
        await _context.Entry(unitProduct).Reference(u => u.User).LoadAsync();
    }
}