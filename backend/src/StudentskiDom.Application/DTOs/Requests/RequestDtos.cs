using System.ComponentModel.DataAnnotations;

namespace StudentskiDom.Application.DTOs.Requests;

public class RequestDto
{
    public Guid Id { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public Guid? RoomId { get; set; }
    public string? RoomNumber { get; set; }
    public Guid? ResourceId { get; set; }
    public string? ResourceName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public int CommentCount { get; set; }
}

public class CreateRequestDto
{
    [Required]
    public string RequestType { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";

    public Guid? RoomId { get; set; }
    public Guid? ResourceId { get; set; }
}

public class UpdateRequestStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}

public class AssignRequestDto
{
    public Guid? AssignedToUserId { get; set; }
}
