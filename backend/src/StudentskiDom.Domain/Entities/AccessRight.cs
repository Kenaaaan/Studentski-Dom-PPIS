using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Domain.Entities;

public class AccessRight
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoomId { get; set; }
    public Guid? ResourceId { get; set; }
    public AccessType AccessType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public Guid GrantedByUserId { get; set; }
    public string? Reason { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Room? Room { get; set; }
    public Resource? Resource { get; set; }
    public User GrantedByUser { get; set; } = null!;
    public ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
}
