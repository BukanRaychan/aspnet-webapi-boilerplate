using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.DTOs.Common;
using WebApi.DTOs.UnitProductDtos;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UnitProductsController : ControllerBase
{
    private readonly IUnitProductService _unitProductService;

    public UnitProductsController(IUnitProductService unitProductService)
    {
        _unitProductService = unitProductService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination)
    {
        PagedResponse<UnitProductResponseDto> unitProducts = await _unitProductService.GetPagedAsync(pagination);
        return Ok(ApiResponseDto<PagedResponse<UnitProductResponseDto>>.SuccessResult(unitProducts, "Unit products retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        UnitProductResponseDto? unitProduct = await _unitProductService.GetByIdAsync(id);

        if (unitProduct == null) {
            return NotFound(ApiResponseDto<UnitProductResponseDto>.ErrorResult($"UnitProduct with id {id} not found", "Not found"));
        }
        
        return Ok(ApiResponseDto<UnitProductResponseDto>.SuccessResult(unitProduct, "Unit product retrieved successfully"));
    }

    [HttpGet("by-product/{productId}")]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        List<UnitProductResponseDto> unitProducts = await _unitProductService.GetByProductIdAsync(productId);
        return Ok(ApiResponseDto<List<UnitProductResponseDto>>.SuccessResult(unitProducts, "Unit products retrieved successfully"));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitProductDto dto)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        UnitProductResponseDto created = await _unitProductService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },ApiResponseDto<UnitProductResponseDto>.SuccessResult(created, "Unit product created successfully"));
    }

    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUnitProductDto dto)
    {
        UnitProductResponseDto? updated = await _unitProductService.UpdateAsync(id, dto);
        
        if (updated == null) {
            return NotFound(ApiResponseDto<UnitProductResponseDto>.ErrorResult(
                $"UnitProduct with id {id} not found", "Not found"));
        }
        
        return Ok(ApiResponseDto<UnitProductResponseDto>.SuccessResult(updated, "Unit product updated successfully"));
    }


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _unitProductService.DeleteAsync(id);
        
        if (!deleted) {
            return NotFound(ApiResponseDto<UnitProductResponseDto>.ErrorResult(
                $"UnitProduct with id {id} not found", "Not found"));
        }

        return NoContent();
    }
}