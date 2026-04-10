namespace StudentskiDom.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Request Request { get; set; } = null!;
    public User AuthorUser { get; set; } = null!;
}
