using StudentskiDom.Application.DTOs.Auth;

namespace StudentskiDom.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<UserDto> GetCurrentUserAsync(Guid userId);
}
