using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.Rooms;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService) => _roomService = roomService;

    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
        => Ok(await _roomService.GetAllRoomsAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id)
        => Ok(await _roomService.GetRoomByIdAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
        => Ok(await _roomService.CreateRoomAsync(dto));

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoomDto>> Update(Guid id, [FromBody] UpdateRoomDto dto)
        => Ok(await _roomService.UpdateRoomAsync(id, dto));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roomService.DeleteRoomAsync(id);
        return NoContent();
    }
}
