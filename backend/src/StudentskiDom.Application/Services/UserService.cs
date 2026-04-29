using StudentskiDom.Application.DTOs.Auth;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _context;

    public UserService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.LastName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                StudentStatus = u.StudentStatus.ToString(),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<UserDto>> GetStaffUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Staff)
            .OrderBy(u => u.LastName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                StudentStatus = u.StudentStatus.ToString(),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<UserDto>> GetStudentUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Student)
            .OrderBy(u => u.LastName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                StudentStatus = u.StudentStatus.ToString(),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found.");

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            StudentStatus = user.StudentStatus.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto> UpdateUserStatusAsync(Guid id, UpdateUserStatusDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found.");

        if (!Enum.TryParse<StudentStatus>(dto.StudentStatus, true, out var status))
            throw new ArgumentException("Invalid student status.");

        user.StudentStatus = status;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            StudentStatus = user.StudentStatus.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto> UpdateUserRoleAsync(Guid id, UpdateUserRoleDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found.");

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            throw new ArgumentException("Invalid role.");

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            StudentStatus = user.StudentStatus.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
