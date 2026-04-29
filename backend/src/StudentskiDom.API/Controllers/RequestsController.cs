using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.Requests;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestsController(IRequestService requestService) => _requestService = requestService;

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<List<RequestDto>>> GetAll(
        [FromQuery] string? status, [FromQuery] string? type, [FromQuery] string? search,
        [FromQuery] string? priority, [FromQuery] Guid? assignedToUserId,
        [FromQuery] Guid? requestedByUserId, [FromQuery] Guid? roomId)
    {
        return Ok(await _requestService.GetAllRequestsAsync(status, type, search, priority, assignedToUserId, requestedByUserId, roomId));
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<RequestDto>>> GetMyRequests([FromQuery] string? status)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _requestService.GetRequestsByUserIdAsync(userId, status));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestDto>> GetById(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        return Ok(await _requestService.GetRequestByIdAsync(id, userId, role));
    }

    [HttpPost]
    public async Task<ActionResult<RequestDto>> Create([FromBody] CreateRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _requestService.CreateRequestAsync(dto, userId));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<RequestDto>> UpdateStatus(Guid id, [FromBody] UpdateRequestStatusDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        return Ok(await _requestService.UpdateRequestStatusAsync(id, dto, userId, role));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}/assign")]
    public async Task<ActionResult<RequestDto>> Assign(Guid id, [FromBody] AssignRequestDto dto)
        => Ok(await _requestService.AssignRequestAsync(id, dto));
}
