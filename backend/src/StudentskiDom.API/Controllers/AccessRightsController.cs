using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.AccessRights;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/access-rights")]
[Authorize]
public class AccessRightsController : ControllerBase
{
    private readonly IAccessRightService _accessRightService;

    public AccessRightsController(IAccessRightService accessRightService) => _accessRightService = accessRightService;

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<List<AccessRightDto>>> GetAll()
        => Ok(await _accessRightService.GetAllAccessRightsAsync());

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<AccessRightDto>>> GetByUserId(Guid userId)
        => Ok(await _accessRightService.GetAccessRightsByUserIdAsync(userId));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AccessRightDto>> Grant([FromBody] GrantAccessDto dto)
    {
        var grantedBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _accessRightService.GrantAccessAsync(dto, grantedBy));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}/revoke")]
    public async Task<ActionResult<AccessRightDto>> Revoke(Guid id)
        => Ok(await _accessRightService.RevokeAccessAsync(id));
}
