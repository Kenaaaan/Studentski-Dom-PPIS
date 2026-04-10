using System.ComponentModel.DataAnnotations;

namespace StudentskiDom.Application.DTOs.AccessRights;

public class AccessRightDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? RoomId { get; set; }
    public string? RoomNumber { get; set; }
    public Guid? ResourceId { get; set; }
    public string? ResourceName { get; set; }
    public string AccessType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string GrantedByName { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class GrantAccessDto
{
    [Required]
    public Guid UserId { get; set; }

    public Guid? RoomId { get; set; }

    public Guid? ResourceId { get; set; }

    [Required]
    public string AccessType { get; set; } = string.Empty;

    public DateTime? ExpiresAt { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }
}

public class AccessLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string AccessAction { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? Details { get; set; }
    public string AccessType { get; set; } = string.Empty;
    public string? RoomOrResourceName { get; set; }
}

public class CreateAccessLogDto
{
    [Required]
    public Guid AccessRightId { get; set; }

    [Required]
    public string AccessAction { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? Details { get; set; }
}
