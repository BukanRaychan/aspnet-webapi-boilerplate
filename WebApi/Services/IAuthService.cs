using WebApi.DTOs.AuthDtos;

namespace WebApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
}
