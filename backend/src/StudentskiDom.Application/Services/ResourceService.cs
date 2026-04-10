using StudentskiDom.Application.DTOs.Resources;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using StudentskiDom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class ResourceService : IResourceService
{
    private readonly IAppDbContext _context;

    public ResourceService(IAppDbContext context) => _context = context;

    public async Task<List<ResourceDto>> GetAllResourcesAsync()
    {
        return await _context.Resources.AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new ResourceDto
            {
                Id = r.Id, Name = r.Name, Description = r.Description,
                ResourceType = r.ResourceType.ToString(), Location = r.Location, IsActive = r.IsActive
            }).ToListAsync();
    }

    public async Task<ResourceDto> GetResourceByIdAsync(Guid id)
    {
        var r = await _context.Resources.FindAsync(id) ?? throw new KeyNotFoundException("Resource not found.");
        return new ResourceDto
        {
            Id = r.Id, Name = r.Name, Description = r.Description,
            ResourceType = r.ResourceType.ToString(), Location = r.Location, IsActive = r.IsActive
        };
    }

    public async Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto)
    {
        if (!Enum.TryParse<ResourceType>(dto.ResourceType, true, out var type))
            throw new ArgumentException("Invalid resource type.");

        var resource = new Resource
        {
            Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description,
            ResourceType = type, Location = dto.Location
        };
        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        return new ResourceDto
        {
            Id = resource.Id, Name = resource.Name, Description = resource.Description,
            ResourceType = resource.ResourceType.ToString(), Location = resource.Location, IsActive = resource.IsActive
        };
    }

    public async Task<ResourceDto> UpdateResourceAsync(Guid id, UpdateResourceDto dto)
    {
        var resource = await _context.Resources.FindAsync(id) ?? throw new KeyNotFoundException("Resource not found.");

        if (dto.Name != null) resource.Name = dto.Name;
        if (dto.Description != null) resource.Description = dto.Description;
        if (dto.ResourceType != null && Enum.TryParse<ResourceType>(dto.ResourceType, true, out var rt)) resource.ResourceType = rt;
        if (dto.Location != null) resource.Location = dto.Location;
        if (dto.IsActive.HasValue) resource.IsActive = dto.IsActive.Value;

        await _context.SaveChangesAsync();
        return new ResourceDto
        {
            Id = resource.Id, Name = resource.Name, Description = resource.Description,
            ResourceType = resource.ResourceType.ToString(), Location = resource.Location, IsActive = resource.IsActive
        };
    }

    public async Task DeleteResourceAsync(Guid id)
    {
        var resource = await _context.Resources.FindAsync(id) ?? throw new KeyNotFoundException("Resource not found.");
        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();
    }
}
