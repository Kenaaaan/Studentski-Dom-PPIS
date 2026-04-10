using Microsoft.EntityFrameworkCore;
using StudentskiDom.Domain.Entities;

namespace StudentskiDom.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Resource> Resources { get; }
    DbSet<AccessRight> AccessRights { get; }
    DbSet<AccessLog> AccessLogs { get; }
    DbSet<Request> Requests { get; }
    DbSet<Comment> Comments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
