using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentskiDom.Application.DTOs.Resources;
using StudentskiDom.Application.Interfaces;

namespace StudentskiDom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService) => _resourceService = resourceService;

    [HttpGet]
    public async Task<ActionResult<List<ResourceDto>>> GetAll()
        => Ok(await _resourceService.GetAllResourcesAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResourceDto>> GetById(Guid id)
        => Ok(await _resourceService.GetResourceByIdAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResourceDto>> Create([FromBody] CreateResourceDto dto)
        => Ok(await _resourceService.CreateResourceAsync(dto));

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ResourceDto>> Update(Guid id, [FromBody] UpdateResourceDto dto)
        => Ok(await _resourceService.UpdateResourceAsync(id, dto));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _resourceService.DeleteResourceAsync(id);
        return NoContent();
    }
}
