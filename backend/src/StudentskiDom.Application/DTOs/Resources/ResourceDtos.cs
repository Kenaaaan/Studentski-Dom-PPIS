using System.ComponentModel.DataAnnotations;

namespace StudentskiDom.Application.DTOs.Resources;

public class ResourceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime? UnavailableUntil { get; set; }
}

public class CreateResourceDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string ResourceType { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Location { get; set; }
}

public class UpdateResourceDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ResourceType { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? UnavailableUntil { get; set; }
}
