using StudentskiDom.Application.DTOs.Rooms;

namespace StudentskiDom.Application.Interfaces;

public interface IRoomService
{
    Task<List<RoomDto>> GetAllRoomsAsync();
    Task<RoomDto> GetRoomByIdAsync(Guid id);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto dto);
    Task<RoomDto> UpdateRoomAsync(Guid id, UpdateRoomDto dto);
    Task DeleteRoomAsync(Guid id);
}
