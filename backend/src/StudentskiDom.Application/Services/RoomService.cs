using StudentskiDom.Application.DTOs.Rooms;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using StudentskiDom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class RoomService : IRoomService
{
    private readonly IAppDbContext _context;

    public RoomService(IAppDbContext context) => _context = context;

    public async Task<List<RoomDto>> GetAllRoomsAsync()
    {
        return await _context.Rooms.AsNoTracking()
            .OrderBy(r => r.Building).ThenBy(r => r.Floor).ThenBy(r => r.RoomNumber)
            .Select(r => new RoomDto
            {
                Id = r.Id, RoomNumber = r.RoomNumber, Floor = r.Floor,
                Building = r.Building, RoomType = r.RoomType.ToString(),
                Capacity = r.Capacity, IsAvailable = r.IsAvailable
            }).ToListAsync();
    }

    public async Task<RoomDto> GetRoomByIdAsync(Guid id)
    {
        var r = await _context.Rooms.FindAsync(id) ?? throw new KeyNotFoundException("Room not found.");
        return new RoomDto
        {
            Id = r.Id, RoomNumber = r.RoomNumber, Floor = r.Floor,
            Building = r.Building, RoomType = r.RoomType.ToString(),
            Capacity = r.Capacity, IsAvailable = r.IsAvailable
        };
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto dto)
    {
        if (!Enum.TryParse<RoomType>(dto.RoomType, true, out var roomType))
            throw new ArgumentException("Invalid room type.");

        var room = new Room
        {
            Id = Guid.NewGuid(), RoomNumber = dto.RoomNumber, Floor = dto.Floor,
            Building = dto.Building, RoomType = roomType, Capacity = dto.Capacity
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomDto
        {
            Id = room.Id, RoomNumber = room.RoomNumber, Floor = room.Floor,
            Building = room.Building, RoomType = room.RoomType.ToString(),
            Capacity = room.Capacity, IsAvailable = room.IsAvailable
        };
    }

    public async Task<RoomDto> UpdateRoomAsync(Guid id, UpdateRoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id) ?? throw new KeyNotFoundException("Room not found.");

        if (dto.RoomNumber != null) room.RoomNumber = dto.RoomNumber;
        if (dto.Floor.HasValue) room.Floor = dto.Floor.Value;
        if (dto.Building != null) room.Building = dto.Building;
        if (dto.RoomType != null && Enum.TryParse<RoomType>(dto.RoomType, true, out var rt)) room.RoomType = rt;
        if (dto.Capacity.HasValue) room.Capacity = dto.Capacity.Value;
        if (dto.IsAvailable.HasValue) room.IsAvailable = dto.IsAvailable.Value;

        await _context.SaveChangesAsync();
        return new RoomDto
        {
            Id = room.Id, RoomNumber = room.RoomNumber, Floor = room.Floor,
            Building = room.Building, RoomType = room.RoomType.ToString(),
            Capacity = room.Capacity, IsAvailable = room.IsAvailable
        };
    }

    public async Task DeleteRoomAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id) ?? throw new KeyNotFoundException("Room not found.");
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }
}
