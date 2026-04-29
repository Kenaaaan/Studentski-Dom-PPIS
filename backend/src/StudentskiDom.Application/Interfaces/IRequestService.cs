using StudentskiDom.Application.DTOs.Requests;

namespace StudentskiDom.Application.Interfaces;

public interface IRequestService
{
    Task<List<RequestDto>> GetAllRequestsAsync(string? status, string? type, string? search, string? priority, Guid? assignedToUserId, Guid? requestedByUserId, Guid? roomId);
    Task<List<RequestDto>> GetRequestsByUserIdAsync(Guid userId, string? status);
    Task<RequestDto> GetRequestByIdAsync(Guid id, Guid userId, string role);
    Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, Guid userId);
    Task<RequestDto> UpdateRequestStatusAsync(Guid id, UpdateRequestStatusDto dto, Guid userId, string role);
    Task<RequestDto> AssignRequestAsync(Guid id, AssignRequestDto dto);
}
