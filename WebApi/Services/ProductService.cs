using AutoMapper;
using ProductCatalogAPI.DTOs.ProductDtos;
using ProductCatalogAPI.Models;
using ProductCatalogAPI.Repositories;

namespace ProductCatalogAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var response = _mapper.Map<List<ProductResponseDto>>(products);
        foreach (var productResponse in response)
        {
            productResponse.Stock = products.First(p => p.Id == productResponse.Id).UnitProducts.Count;
        }
        return response;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        var productResponse = _mapper.Map<ProductResponseDto>(product);
        productResponse.Stock = await _productRepository.GetStockCountAsync(id);
        return productResponse;
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;
        var created = await _productRepository.CreateAsync(product);
        return _mapper.Map<ProductResponseDto>(created);
    }

    public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null) return null;
        _mapper.Map(dto, existing);
        var updated = await _productRepository.UpdateAsync(existing);
        return _mapper.Map<ProductResponseDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }
}