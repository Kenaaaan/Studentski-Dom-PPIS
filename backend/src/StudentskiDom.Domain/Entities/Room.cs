using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string Building { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
