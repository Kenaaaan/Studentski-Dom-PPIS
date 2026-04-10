using System.ComponentModel.DataAnnotations;

namespace StudentskiDom.Application.DTOs.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCommentDto
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    public bool IsInternal { get; set; } = false;
}
