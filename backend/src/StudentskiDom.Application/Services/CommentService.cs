using StudentskiDom.Application.DTOs.Comments;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudentskiDom.Application.Services;

public class CommentService : ICommentService
{
    private readonly IAppDbContext _context;

    public CommentService(IAppDbContext context) => _context = context;

    public async Task<List<CommentDto>> GetCommentsByRequestIdAsync(Guid requestId, bool includeInternal)
    {
        var query = _context.Comments
            .AsNoTracking()
            .Include(c => c.AuthorUser)
            .Where(c => c.RequestId == requestId);

        if (!includeInternal)
            query = query.Where(c => !c.IsInternal);

        return await query.OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                RequestId = c.RequestId,
                AuthorUserId = c.AuthorUserId,
                AuthorName = c.AuthorUser.FirstName + " " + c.AuthorUser.LastName,
                AuthorRole = c.AuthorUser.Role.ToString(),
                Content = c.Content,
                IsInternal = c.IsInternal,
                CreatedAt = c.CreatedAt
            }).ToListAsync();
    }

    public async Task<CommentDto> CreateCommentAsync(Guid requestId, CreateCommentDto dto, Guid authorUserId)
    {
        _ = await _context.Requests.FindAsync(requestId)
            ?? throw new KeyNotFoundException("Request not found.");

        var user = await _context.Users.FindAsync(authorUserId)
            ?? throw new KeyNotFoundException("User not found.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            AuthorUserId = authorUserId,
            Content = dto.Content,
            IsInternal = dto.IsInternal,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return new CommentDto
        {
            Id = comment.Id,
            RequestId = comment.RequestId,
            AuthorUserId = comment.AuthorUserId,
            AuthorName = $"{user.FirstName} {user.LastName}",
            AuthorRole = user.Role.ToString(),
            Content = comment.Content,
            IsInternal = comment.IsInternal,
            CreatedAt = comment.CreatedAt
        };
    }
}
