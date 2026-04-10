using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Domain.Entities;

public class Resource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ResourceType ResourceType { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();
}
