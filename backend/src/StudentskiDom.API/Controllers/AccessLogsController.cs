using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.AccessRights;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/access-logs")]
[Authorize]
public class AccessLogsController : ControllerBase
{
    private readonly IAccessLogService _accessLogService;

    public AccessLogsController(IAccessLogService accessLogService) => _accessLogService = accessLogService;

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<List<AccessLogDto>>> GetAll(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? userId)
        => Ok(await _accessLogService.GetAllAccessLogsAsync(from, to, userId));

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<AccessLogDto>>> GetByUserId(Guid userId)
        => Ok(await _accessLogService.GetAccessLogsByUserIdAsync(userId));

    [HttpPost]
    public async Task<ActionResult<AccessLogDto>> Create([FromBody] CreateAccessLogDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _accessLogService.CreateAccessLogAsync(dto, userId));
    }
}
