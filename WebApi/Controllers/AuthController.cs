using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalogAPI.DTOs;
using ProductCatalogAPI.DTOs.AuthDtos;
using ProductCatalogAPI.Services;


namespace ProductCatalogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result == null)
            return BadRequest(ApiResponseDto<object>.ErrorResult(
                "Email already exists or registration failed",
                "Registration failed"));

        return Ok(ApiResponseDto<object>.SuccessResult(result, "Registration successful"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized(ApiResponseDto<object>.ErrorResult(
                "Invalid email or password",
                "Login failed"));

        return Ok(ApiResponseDto<object>.SuccessResult(result, "Login successful"));
    }

    [Authorize]
    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponseDto<object>.ErrorResult(
                "Invalid Token",
                "Unauthorized"));

        var result = await _authService.UpdateProfileAsync(userId, dto);

        if (!result)
            return BadRequest(ApiResponseDto<object>.ErrorResult(
                "Update failed — check your current password",
                "Update failed"));

        return Ok(ApiResponseDto<object>.SuccessResult(null, "Profile updated successfully"));
    }
}
