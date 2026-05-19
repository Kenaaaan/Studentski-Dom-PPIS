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

    public async Task<List<RequestDto>> GetAssignedRequestsAsync(Guid userId)
    {
        var requests = await _context.Requests
            .AsNoTracking()
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .Where(r => r.AssignedToUserId == userId)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();
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

        if (role != "Admin" && request.RequestedByUserId != userId && request.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to view this request.");

        return MapToDto(request);
    }

    public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, Guid userId)
    {
        if (!Enum.TryParse<RequestType>(dto.RequestType, true, out var requestType))
            throw new ArgumentException("Nepoznat tip zahtjeva.");

        if (!Enum.TryParse<Priority>(dto.Priority, true, out var priority))
            priority = Priority.Medium;

        await ValidateNoDuplicateAsync(requestType, dto, userId);

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

        if (role != "Admin" && request.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("Možete ažurirati samo status zahtjeva koji su vam dodijeljeni.");

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
                    AccessType = request.ResourceId.HasValue
                        ? (request.Resource?.ResourceType == ResourceType.Network ? AccessType.Network : AccessType.CommonArea)
                        : AccessType.Room,
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

        // Reload with all navigation props
        request = await _context.Requests
            .Include(r => r.RequestedByUser)
            .Include(r => r.AssignedToUser)
            .Include(r => r.Room)
            .Include(r => r.Resource)
            .Include(r => r.Comments)
            .FirstAsync(r => r.Id == id);

        return MapToDto(request);
    }

    private async Task ValidateNoDuplicateAsync(RequestType requestType, CreateRequestDto dto, Guid userId)
    {
        if (requestType == RequestType.AccessRequest)
        {
            if (dto.ResourceId.HasValue)
            {
                // Block if real active access right exists in DB
                var hasActiveRight = await _context.AccessRights
                    .AnyAsync(ar => ar.UserId == userId && ar.ResourceId == dto.ResourceId && ar.IsActive);
                if (hasActiveRight)
                    throw new InvalidOperationException("Već imate aktivan pristup ovom resursu.");

                // Block if resource is a default type (all students get it automatically)
                var defaultTypes = new[] { ResourceType.Network, ResourceType.StudyRoom, ResourceType.Gym, ResourceType.Kitchen, ResourceType.CommonRoom };
                var user = await _context.Users.FindAsync(userId);
                if (user?.Role == UserRole.Student)
                {
                    var resource = await _context.Resources.FindAsync(dto.ResourceId.Value);
                    if (resource != null && defaultTypes.Contains(resource.ResourceType))
                        throw new InvalidOperationException("Pristup ovom resursu je automatski dodijeljen svim stanarima.");
                }

                // Block if already has pending/in-progress AccessRequest for this resource
                var hasPending = await _context.Requests
                    .AnyAsync(r => r.RequestedByUserId == userId
                        && r.RequestType == RequestType.AccessRequest
                        && r.ResourceId == dto.ResourceId
                        && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress));
                if (hasPending)
                    throw new InvalidOperationException("Već imate aktivan zahtjev za ovaj resurs.");
            }

            if (dto.RoomId.HasValue)
            {
                var hasActiveRoomRight = await _context.AccessRights
                    .AnyAsync(ar => ar.UserId == userId && ar.RoomId == dto.RoomId && ar.IsActive);
                if (hasActiveRoomRight)
                    throw new InvalidOperationException("Već imate aktivan pristup ovoj sobi.");

                var hasPending = await _context.Requests
                    .AnyAsync(r => r.RequestedByUserId == userId
                        && r.RequestType == RequestType.AccessRequest
                        && r.RoomId == dto.RoomId
                        && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress));
                if (hasPending)
                    throw new InvalidOperationException("Već imate aktivan zahtjev za ovu sobu.");
            }
        }

        // Only one RoomChange request at a time
        if (requestType == RequestType.RoomChange)
        {
            var hasActivePending = await _context.Requests
                .AnyAsync(r => r.RequestedByUserId == userId
                    && r.RequestType == RequestType.RoomChange
                    && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress));
            if (hasActivePending)
                throw new InvalidOperationException("Već imate aktivan zahtjev za zamjenu sobe. Sačekajte da bude riješen.");
        }

        // Only one ParkingPermit request at a time
        if (requestType == RequestType.ParkingPermit)
        {
            var hasActivePending = await _context.Requests
                .AnyAsync(r => r.RequestedByUserId == userId
                    && r.RequestType == RequestType.ParkingPermit
                    && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress));
            if (hasActivePending)
                throw new InvalidOperationException("Već imate aktivan zahtjev za dozvolu parkinga.");
        }

        // Only one StorageRequest at a time
        if (requestType == RequestType.StorageRequest)
        {
            var hasActivePending = await _context.Requests
                .AnyAsync(r => r.RequestedByUserId == userId
                    && r.RequestType == RequestType.StorageRequest
                    && (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress));
            if (hasActivePending)
                throw new InvalidOperationException("Već imate aktivan zahtjev za ostavu.");
        }
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
