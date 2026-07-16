using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.DTOs.Common;
using WebApi.DTOs.ProductDtos;
using WebApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination)
    {
        PagedResponse<ProductResponseDto> products = await _productService.GetPagedAsync(pagination);
        
        return Ok(ApiResponseDto<PagedResponse<ProductResponseDto>>.SuccessResult(products, "Products retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    { 
        ProductResponseDto? product = await _productService.GetByIdAsync(id);

        if (product == null) {
            return NotFound($"Product with id {id} not found");
        }

        return Ok(ApiResponseDto<ProductResponseDto>.SuccessResult(product, "Product retrieved successfully"));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        ProductResponseDto created = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponseDto<ProductResponseDto>.SuccessResult(created, "Product created successfully"));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        ProductResponseDto? updated = await _productService.UpdateAsync(id, dto);
        
        if (updated == null) {
            return NotFound(ApiResponseDto<ProductResponseDto>.ErrorResult($"Product with id {id} not found", "Not found"));
        
        }
        
        return Ok(ApiResponseDto<ProductResponseDto>.SuccessResult(updated, "Product updated successfully"));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _productService.DeleteAsync(id);
        
        if (!deleted) {
            return NotFound(ApiResponseDto<ProductResponseDto>.ErrorResult($"Product with id {id} not found", "Not found"));
        }
        
        return NoContent();
    }
}