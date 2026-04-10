using StudentskiDom.Application.DTOs.Auth;

namespace StudentskiDom.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(Guid id);
    Task<UserDto> UpdateUserStatusAsync(Guid id, UpdateUserStatusDto dto);
    Task<UserDto> UpdateUserRoleAsync(Guid id, UpdateUserRoleDto dto);
}
