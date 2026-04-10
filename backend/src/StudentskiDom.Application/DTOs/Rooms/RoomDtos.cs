using System.ComponentModel.DataAnnotations;

namespace StudentskiDom.Application.DTOs.Rooms;

public class RoomDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Floor { get; set; }
    public string Building { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreateRoomDto
{
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [Range(0, 20)]
    public int Floor { get; set; }

    [Required]
    [StringLength(50)]
    public string Building { get; set; } = string.Empty;

    [Required]
    public string RoomType { get; set; } = string.Empty;

    [Required]
    [Range(1, 6)]
    public int Capacity { get; set; }
}

public class UpdateRoomDto
{
    [StringLength(20)]
    public string? RoomNumber { get; set; }

    [Range(0, 20)]
    public int? Floor { get; set; }

    [StringLength(50)]
    public string? Building { get; set; }

    public string? RoomType { get; set; }

    [Range(1, 6)]
    public int? Capacity { get; set; }

    public bool? IsAvailable { get; set; }
}
