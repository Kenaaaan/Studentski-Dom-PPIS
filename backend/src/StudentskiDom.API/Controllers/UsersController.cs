using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.Auth;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<List<UserDto>>> GetAll()
        => Ok(await _userService.GetAllUsersAsync());

    [HttpGet("staff")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserDto>>> GetStaff()
        => Ok(await _userService.GetStaffUsersAsync());

    [HttpGet("students")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<List<UserDto>>> GetStudents()
        => Ok(await _userService.GetStudentUsersAsync());

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
        => Ok(await _userService.GetUserByIdAsync(id));

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
        => Ok(await _userService.UpdateUserStatusAsync(id, dto));

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateRole(Guid id, [FromBody] UpdateUserRoleDto dto)
        => Ok(await _userService.UpdateUserRoleAsync(id, dto));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
