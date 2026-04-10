using StudentskiDom.Application.DTOs.Resources;

namespace StudentskiDom.Application.Interfaces;

public interface IResourceService
{
    Task<List<ResourceDto>> GetAllResourcesAsync();
    Task<ResourceDto> GetResourceByIdAsync(Guid id);
    Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto);
    Task<ResourceDto> UpdateResourceAsync(Guid id, UpdateResourceDto dto);
    Task DeleteResourceAsync(Guid id);
}
