using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Domain.Entities;

public class AccessLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AccessRightId { get; set; }
    public AccessAction AccessAction { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? Details { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public AccessRight AccessRight { get; set; } = null!;
}
