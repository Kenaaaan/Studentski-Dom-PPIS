using StudentskiDom.Application.DTOs.AccessRights;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using StudentskiDom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class AccessRightService : IAccessRightService
{
    private readonly IAppDbContext _context;

    public AccessRightService(IAppDbContext context) => _context = context;

    public async Task<List<AccessRightDto>> GetAllAccessRightsAsync()
    {
        return await _context.AccessRights
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Room)
            .Include(a => a.Resource)
            .Include(a => a.GrantedByUser)
            .OrderByDescending(a => a.GrantedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<List<AccessRightDto>> GetAccessRightsByUserIdAsync(Guid userId)
    {
        return await _context.AccessRights
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Room)
            .Include(a => a.Resource)
            .Include(a => a.GrantedByUser)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.GrantedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
    }

    public async Task<AccessRightDto> GrantAccessAsync(GrantAccessDto dto, Guid grantedByUserId)
    {
        if (!Enum.TryParse<AccessType>(dto.AccessType, true, out var accessType))
            throw new ArgumentException("Invalid access type.");

        // Validate user exists
        var user = await _context.Users.FindAsync(dto.UserId)
            ?? throw new KeyNotFoundException("User not found.");

        // Validate room/resource exists if specified
        if (dto.RoomId.HasValue)
        {
            _ = await _context.Rooms.FindAsync(dto.RoomId.Value)
                ?? throw new KeyNotFoundException("Room not found.");
        }
        if (dto.ResourceId.HasValue)
        {
            _ = await _context.Resources.FindAsync(dto.ResourceId.Value)
                ?? throw new KeyNotFoundException("Resource not found.");
        }

        var accessRight = new AccessRight
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            RoomId = dto.RoomId,
            ResourceId = dto.ResourceId,
            AccessType = accessType,
            IsActive = true,
            GrantedAt = DateTime.UtcNow,
            ExpiresAt = dto.ExpiresAt,
            GrantedByUserId = grantedByUserId,
            Reason = dto.Reason
        };

        _context.AccessRights.Add(accessRight);
        await _context.SaveChangesAsync();

        // Reload with includes
        var result = await _context.AccessRights
            .Include(a => a.User)
            .Include(a => a.Room)
            .Include(a => a.Resource)
            .Include(a => a.GrantedByUser)
            .FirstAsync(a => a.Id == accessRight.Id);

        return MapToDto(result);
    }

    public async Task<AccessRightDto> RevokeAccessAsync(Guid id)
    {
        var accessRight = await _context.AccessRights
            .Include(a => a.User)
            .Include(a => a.Room)
            .Include(a => a.Resource)
            .Include(a => a.GrantedByUser)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException("Access right not found.");

        accessRight.IsActive = false;
        await _context.SaveChangesAsync();

        return MapToDto(accessRight);
    }

    private static AccessRightDto MapToDto(AccessRight a) => new()
    {
        Id = a.Id,
        UserId = a.UserId,
        UserName = $"{a.User.FirstName} {a.User.LastName}",
        RoomId = a.RoomId,
        RoomNumber = a.Room?.RoomNumber,
        ResourceId = a.ResourceId,
        ResourceName = a.Resource?.Name,
        AccessType = a.AccessType.ToString(),
        IsActive = a.IsActive,
        GrantedAt = a.GrantedAt,
        ExpiresAt = a.ExpiresAt,
        GrantedByName = $"{a.GrantedByUser.FirstName} {a.GrantedByUser.LastName}",
        Reason = a.Reason
    };
}
