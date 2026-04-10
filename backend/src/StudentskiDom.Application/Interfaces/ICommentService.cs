using StudentskiDom.Application.DTOs.Comments;

namespace StudentskiDom.Application.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsByRequestIdAsync(Guid requestId, bool includeInternal);
    Task<CommentDto> CreateCommentAsync(Guid requestId, CreateCommentDto dto, Guid authorUserId);
}
