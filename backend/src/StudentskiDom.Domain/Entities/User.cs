using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public StudentStatus StudentStatus { get; set; } = StudentStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();
    public ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();
    public ICollection<Request> SubmittedRequests { get; set; } = new List<Request>();
    public ICollection<Request> AssignedRequests { get; set; } = new List<Request>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<AccessRight> GrantedAccessRights { get; set; } = new List<AccessRight>();
}
