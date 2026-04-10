using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Domain.Entities;

public class Request
{
    public Guid Id { get; set; }
    public Guid RequestedByUserId { get; set; }
    public RequestType RequestType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public Guid? AssignedToUserId { get; set; }
    public Guid? RoomId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public User RequestedByUser { get; set; } = null!;
    public User? AssignedToUser { get; set; }
    public Room? Room { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
