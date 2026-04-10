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
        [FromQuery] string? status, [FromQuery] string? type, [FromQuery] string? search)
        => Ok(await _requestService.GetAllRequestsAsync(status, type, search));

    [HttpGet("my")]
    public async Task<ActionResult<List<RequestDto>>> GetMyRequests([FromQuery] string? status)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _requestService.GetRequestsByUserIdAsync(userId, status));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestDto>> GetById(Guid id)
        => Ok(await _requestService.GetRequestByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<RequestDto>> Create([FromBody] CreateRequestDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _requestService.CreateRequestAsync(dto, userId));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<RequestDto>> UpdateStatus(Guid id, [FromBody] UpdateRequestStatusDto dto)
        => Ok(await _requestService.UpdateRequestStatusAsync(id, dto));

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}/assign")]
    public async Task<ActionResult<RequestDto>> Assign(Guid id, [FromBody] AssignRequestDto dto)
        => Ok(await _requestService.AssignRequestAsync(id, dto));
}
