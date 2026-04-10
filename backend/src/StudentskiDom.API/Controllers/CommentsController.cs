using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.Comments;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/requests/{requestId:guid}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService) => _commentService = commentService;

    [HttpGet]
    public async Task<ActionResult<List<CommentDto>>> GetComments(Guid requestId)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var includeInternal = role == "Admin" || role == "Staff";
        return Ok(await _commentService.GetCommentsByRequestIdAsync(requestId, includeInternal));
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(Guid requestId, [FromBody] CreateCommentDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _commentService.CreateCommentAsync(requestId, dto, userId));
    }
}
