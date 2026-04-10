using StudentskiDom.Application.DTOs.AccessRights;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using StudentskiDom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class AccessLogService : IAccessLogService
{
    private readonly IAppDbContext _context;

    public AccessLogService(IAppDbContext context) => _context = context;

    public async Task<List<AccessLogDto>> GetAllAccessLogsAsync(DateTime? from, DateTime? to, Guid? userId)
    {
        var query = _context.AccessLogs
            .AsNoTracking()
            .Include(l => l.User)
            .Include(l => l.AccessRight)
                .ThenInclude(a => a.Room)
            .Include(l => l.AccessRight)
                .ThenInclude(a => a.Resource)
            .AsQueryable();

        if (from.HasValue) query = query.Where(l => l.Timestamp >= from.Value);
        if (to.HasValue) query = query.Where(l => l.Timestamp <= to.Value);
        if (userId.HasValue) query = query.Where(l => l.UserId == userId.Value);

        return await query.OrderByDescending(l => l.Timestamp)
            .Take(500)
            .Select(l => new AccessLogDto
            {
                Id = l.Id,
                UserId = l.UserId,
                UserName = l.User.FirstName + " " + l.User.LastName,
                AccessAction = l.AccessAction.ToString(),
                Timestamp = l.Timestamp,
                IpAddress = l.IpAddress,
                Details = l.Details,
                AccessType = l.AccessRight.AccessType.ToString(),
                RoomOrResourceName = l.AccessRight.Room != null ? l.AccessRight.Room.RoomNumber : (l.AccessRight.Resource != null ? l.AccessRight.Resource.Name : null)
            }).ToListAsync();
    }

    public async Task<List<AccessLogDto>> GetAccessLogsByUserIdAsync(Guid userId)
    {
        return await GetAllAccessLogsAsync(null, null, userId);
    }

    public async Task<AccessLogDto> CreateAccessLogAsync(CreateAccessLogDto dto, Guid userId)
    {
        if (!Enum.TryParse<AccessAction>(dto.AccessAction, true, out var action))
            throw new ArgumentException("Invalid access action.");

        var accessRight = await _context.AccessRights
            .Include(a => a.Room)
            .Include(a => a.Resource)
            .FirstOrDefaultAsync(a => a.Id == dto.AccessRightId)
            ?? throw new KeyNotFoundException("Access right not found.");

        if (!accessRight.IsActive)
            throw new InvalidOperationException("Access right is not active.");

        var log = new AccessLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccessRightId = dto.AccessRightId,
            AccessAction = action,
            Timestamp = DateTime.UtcNow,
            IpAddress = dto.IpAddress,
            Details = dto.Details
        };

        _context.AccessLogs.Add(log);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);
        return new AccessLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = $"{user!.FirstName} {user.LastName}",
            AccessAction = log.AccessAction.ToString(),
            Timestamp = log.Timestamp,
            IpAddress = log.IpAddress,
            Details = log.Details,
            AccessType = accessRight.AccessType.ToString(),
            RoomOrResourceName = accessRight.Room?.RoomNumber ?? accessRight.Resource?.Name
        };
    }
}
