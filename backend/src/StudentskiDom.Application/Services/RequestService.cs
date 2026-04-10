using StudentskiDom.Application.DTOs.Requests;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using StudentskiDom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class RequestService : IRequestService
{
    private readonly IAppDbContext _context;

    public RequestService(IAppDbContext context) => _context = context;

    public async Task<List<RequestDto>> GetAllRequestsAsync(string? status, string? type, string? search)
    {
        var query = _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<RequestStatus>(status, true, out var s))
            query = query.Where(r => r.Status == s);

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<RequestType>(type, true, out var t))
            query = query.Where(r => r.RequestType == t);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Title.Contains(search) || r.Description.Contains(search));

        return await query.OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<List<RequestDto>> GetRequestsByUserIdAsync(Guid userId, string? status)
    {
        var query = _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .Where(r => r.RequestedByUserId == userId);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<RequestStatus>(status, true, out var s))
            query = query.Where(r => r.Status == s);

        return await query.OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<RequestDto> GetRequestByIdAsync(Guid id)
    {
        var request = await _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Request not found.");

        return MapToDto(request);
    }

    public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, Guid userId)
    {
        if (!Enum.TryParse<RequestType>(dto.RequestType, true, out var requestType))
            throw new ArgumentException("Invalid request type.");

        if (!Enum.TryParse<Priority>(dto.Priority, true, out var priority))
            priority = Priority.Medium;

        var request = new Request
        {
            Id = Guid.NewGuid(),
            RequestedByUserId = userId,
            RequestType = requestType,
            Title = dto.Title,
            Description = dto.Description,
            Status = RequestStatus.Pending,
            Priority = priority,
            RoomId = dto.RoomId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Requests.Add(request);
        await _context.SaveChangesAsync();

        // Reload with navigation props
        var result = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .FirstAsync(r => r.Id == request.Id);

        return MapToDto(result);
    }

    public async Task<RequestDto> UpdateRequestStatusAsync(Guid id, UpdateRequestStatusDto dto)
    {
        if (!Enum.TryParse<RequestStatus>(dto.Status, true, out var status))
            throw new ArgumentException("Invalid status.");

        var request = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Request not found.");

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;

        if (status == RequestStatus.Resolved)
            request.ResolvedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(request);
    }

    public async Task<RequestDto> AssignRequestAsync(Guid id, AssignRequestDto dto)
    {
        var request = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Request not found.");

        _ = await _context.Users.FindAsync(dto.AssignedToUserId)
            ?? throw new KeyNotFoundException("Assigned user not found.");

        request.AssignedToUserId = dto.AssignedToUserId;
        request.UpdatedAt = DateTime.UtcNow;

        if (request.Status == RequestStatus.Pending)
            request.Status = RequestStatus.InProgress;

        await _context.SaveChangesAsync();

        // Reload assigned user
        request = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Comments)
            .FirstAsync(r => r.Id == id);

        return MapToDto(request);
    }

    private static RequestDto MapToDto(Request r) => new()
    {
        Id = r.Id,
        RequestedByUserId = r.RequestedByUserId,
        RequestedByName = $"{r.RequestedByUser.FirstName} {r.RequestedByUser.LastName}",
        RequestType = r.RequestType.ToString(),
        Title = r.Title,
        Description = r.Description,
        Status = r.Status.ToString(),
        Priority = r.Priority.ToString(),
        AssignedToUserId = r.AssignedToUserId,
        AssignedToName = r.AssignedToUser != null ? $"{r.AssignedToUser.FirstName} {r.AssignedToUser.LastName}" : null,
        RoomId = r.RoomId,
        RoomNumber = r.Room?.RoomNumber,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        ResolvedAt = r.ResolvedAt,
        CommentCount = r.Comments.Count
    };
}
