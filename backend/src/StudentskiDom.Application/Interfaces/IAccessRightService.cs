using StudentskiDom.Application.DTOs.AccessRights;

namespace StudentskiDom.Application.Interfaces;

public interface IAccessRightService
{
    Task<List<AccessRightDto>> GetAllAccessRightsAsync();
    Task<List<AccessRightDto>> GetAccessRightsByUserIdAsync(Guid userId);
    Task<AccessRightDto> GrantAccessAsync(GrantAccessDto dto, Guid grantedByUserId);
    Task<AccessRightDto> RevokeAccessAsync(Guid id);
}

public interface IAccessLogService
{
    Task<List<AccessLogDto>> GetAllAccessLogsAsync(DateTime? from, DateTime? to, Guid? userId);
    Task<List<AccessLogDto>> GetAccessLogsByUserIdAsync(Guid userId);
    Task<AccessLogDto> CreateAccessLogAsync(CreateAccessLogDto dto, Guid userId);
}
