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

    public async Task<List<RequestDto>> GetAllRequestsAsync(string? status, string? type, string? search, string? priority, Guid? assignedToUserId, Guid? requestedByUserId, Guid? roomId)
    {
        var query = _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<RequestStatus>(status, true, out var s))
            query = query.Where(r => r.Status == s);

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<RequestType>(type, true, out var t))
            query = query.Where(r => r.RequestType == t);

        if (!string.IsNullOrEmpty(priority) && Enum.TryParse<Priority>(priority, true, out var p))
            query = query.Where(r => r.Priority == p);

        if (assignedToUserId.HasValue)
            query = query.Where(r => r.AssignedToUserId == assignedToUserId.Value);

        if (requestedByUserId.HasValue)
            query = query.Where(r => r.RequestedByUserId == requestedByUserId.Value);

        if (roomId.HasValue)
            query = query.Where(r => r.RoomId == roomId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Title.Contains(search) || r.Description.Contains(search));

        var requests = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return requests.Select(MapToDto).ToList();
    }

    public async Task<List<RequestDto>> GetRequestsByUserIdAsync(Guid userId, string? status)
    {
        var query = _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .Where(r => r.RequestedByUserId == userId);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<RequestStatus>(status, true, out var s))
            query = query.Where(r => r.Status == s);

        var requests = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return requests.Select(MapToDto).ToList();
    }

    public async Task<RequestDto> GetRequestByIdAsync(Guid id, Guid userId, string role)
    {
        var request = await _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Request not found.");

        if (role != "Admin" && role != "Staff" && request.RequestedByUserId != userId && request.AssignedToUserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this request.");
        }

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
            ResourceId = dto.ResourceId,
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
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .FirstAsync(r => r.Id == request.Id);

        return MapToDto(result);
    }

    public async Task<RequestDto> UpdateRequestStatusAsync(Guid id, UpdateRequestStatusDto dto, Guid userId, string role)
    {
        if (!Enum.TryParse<RequestStatus>(dto.Status, true, out var status))
            throw new ArgumentException("Invalid status.");

        var request = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Request not found.");

        if (role != "Admin" && role != "Staff" && request.AssignedToUserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update the status of this request.");
        }

        request.Status = status;
        request.UpdatedAt = DateTime.UtcNow;

        if (status == RequestStatus.Resolved)
        {
            request.ResolvedAt = DateTime.UtcNow;

            // Automatika za AccessRequest
            if (request.RequestType == RequestType.AccessRequest)
            {
                var accessRight = new AccessRight
                {
                    Id = Guid.NewGuid(),
                    UserId = request.RequestedByUserId,
                    RoomId = request.RoomId,
                    ResourceId = request.ResourceId,
                    AccessType = request.ResourceId.HasValue ? AccessType.CommonArea : AccessType.Room,
                    IsActive = true,
                    GrantedAt = DateTime.UtcNow,
                    GrantedByUserId = userId,
                    Reason = $"Automatski dodijeljeno na osnovu zahtjeva: {request.Title}"
                };
                _context.AccessRights.Add(accessRight);
            }
        }

        await _context.SaveChangesAsync();
        return MapToDto(request);
    }

    public async Task<RequestDto> AssignRequestAsync(Guid id, AssignRequestDto dto)
    {
        var request = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException("Request not found.");

        if (dto.AssignedToUserId.HasValue)
        {
            _ = await _context.Users.FindAsync(dto.AssignedToUserId.Value)
                ?? throw new KeyNotFoundException("Assigned user not found.");

            request.AssignedToUserId = dto.AssignedToUserId;
            if (request.Status == RequestStatus.Pending)
                request.Status = RequestStatus.InProgress;
        }
        else
        {
            request.AssignedToUserId = null;
        }

        request.UpdatedAt = DateTime.UtcNow;

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
        ResourceId = r.ResourceId,
        ResourceName = r.Resource?.Name,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        ResolvedAt = r.ResolvedAt,
        CommentCount = r.Comments.Count
    };
}
