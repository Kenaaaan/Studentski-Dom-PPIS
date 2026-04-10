using Microsoft.EntityFrameworkCore;
using StudentskiDom.Application.Interfaces;
using StudentskiDom.Domain.Entities;
using StudentskiDom.Domain.Enums;

namespace StudentskiDom.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<AccessRight> AccessRights => Set<AccessRight>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasConversion<int>();
            entity.Property(e => e.StudentStatus).HasConversion<int>();
        });

        // Room
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RoomNumber).IsUnique();
            entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Building).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RoomType).HasConversion<int>();
        });

        // Resource
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.ResourceType).HasConversion<int>();
        });

        // AccessRight
        modelBuilder.Entity<AccessRight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccessType).HasConversion<int>();
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.AccessRights)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.AccessRights)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Resource)
                .WithMany(r => r.AccessRights)
                .HasForeignKey(e => e.ResourceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.GrantedByUser)
                .WithMany(u => u.GrantedAccessRights)
                .HasForeignKey(e => e.GrantedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AccessLog
        modelBuilder.Entity<AccessLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccessAction).HasConversion<int>();
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.Details).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.AccessLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AccessRight)
                .WithMany(a => a.AccessLogs)
                .HasForeignKey(e => e.AccessRightId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Request
        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestType).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Priority).HasConversion<int>();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);

            entity.HasOne(e => e.RequestedByUser)
                .WithMany(u => u.SubmittedRequests)
                .HasForeignKey(e => e.RequestedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedToUser)
                .WithMany(u => u.AssignedRequests)
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Room)
                .WithMany(r => r.Requests)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);

            entity.HasOne(e => e.Request)
                .WithMany(r => r.Comments)
                .HasForeignKey(e => e.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AuthorUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.AuthorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
        var staffId = Guid.Parse("b2222222-2222-2222-2222-222222222222");
        var studentId = Guid.Parse("c3333333-3333-3333-3333-333333333333");

        // Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId, FirstName = "Admin", LastName = "Adminović",
                Email = "admin@studentskidom.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                PhoneNumber = "+38761000001", Role = UserRole.Admin,
                StudentStatus = StudentStatus.Active,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = staffId, FirstName = "Tehničar", LastName = "Tehničarević",
                Email = "staff@studentskidom.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                PhoneNumber = "+38761000002", Role = UserRole.Staff,
                StudentStatus = StudentStatus.Active,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = studentId, FirstName = "Student", LastName = "Studentović",
                Email = "student@studentskidom.ba",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                PhoneNumber = "+38761000003", Role = UserRole.Student,
                StudentStatus = StudentStatus.Active,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // Rooms
        var room1Id = Guid.Parse("d4444444-4444-4444-4444-444444444441");
        var room2Id = Guid.Parse("d4444444-4444-4444-4444-444444444442");
        var room3Id = Guid.Parse("d4444444-4444-4444-4444-444444444443");
        var room4Id = Guid.Parse("d4444444-4444-4444-4444-444444444444");
        var room5Id = Guid.Parse("d4444444-4444-4444-4444-444444444445");

        modelBuilder.Entity<Room>().HasData(
            new Room { Id = room1Id, RoomNumber = "A-101", Floor = 1, Building = "Paviljon A", RoomType = RoomType.Double, Capacity = 2, IsAvailable = true },
            new Room { Id = room2Id, RoomNumber = "A-102", Floor = 1, Building = "Paviljon A", RoomType = RoomType.Double, Capacity = 2, IsAvailable = true },
            new Room { Id = room3Id, RoomNumber = "A-201", Floor = 2, Building = "Paviljon A", RoomType = RoomType.Single, Capacity = 1, IsAvailable = false },
            new Room { Id = room4Id, RoomNumber = "B-101", Floor = 1, Building = "Paviljon B", RoomType = RoomType.Triple, Capacity = 3, IsAvailable = true },
            new Room { Id = room5Id, RoomNumber = "B-201", Floor = 2, Building = "Paviljon B", RoomType = RoomType.Double, Capacity = 2, IsAvailable = true }
        );

        // Resources
        var res1Id = Guid.Parse("e5555555-5555-5555-5555-555555555551");
        var res2Id = Guid.Parse("e5555555-5555-5555-5555-555555555552");
        var res3Id = Guid.Parse("e5555555-5555-5555-5555-555555555553");
        var res4Id = Guid.Parse("e5555555-5555-5555-5555-555555555554");

        modelBuilder.Entity<Resource>().HasData(
            new Resource { Id = res1Id, Name = "Kuhinja - Paviljon A", Description = "Zajednička kuhinja na prvom spratu", ResourceType = ResourceType.Kitchen, Location = "Paviljon A, 1. sprat", IsActive = true },
            new Resource { Id = res2Id, Name = "Teretana", Description = "Fitness centar za studente", ResourceType = ResourceType.Gym, Location = "Paviljon B, prizemlje", IsActive = true },
            new Resource { Id = res3Id, Name = "Učionica 1", Description = "Učionica za grupno učenje", ResourceType = ResourceType.StudyRoom, Location = "Paviljon A, 3. sprat", IsActive = true },
            new Resource { Id = res4Id, Name = "WiFi Mreža", Description = "Pristup internet mreži u domu", ResourceType = ResourceType.Network, Location = "Cijeli dom", IsActive = true }
        );

        // Access Rights
        var ar1Id = Guid.Parse("f6666666-6666-6666-6666-666666666661");
        var ar2Id = Guid.Parse("f6666666-6666-6666-6666-666666666662");
        var ar3Id = Guid.Parse("f6666666-6666-6666-6666-666666666663");

        modelBuilder.Entity<AccessRight>().HasData(
            new AccessRight
            {
                Id = ar1Id, UserId = studentId, RoomId = room1Id, AccessType = AccessType.Room,
                IsActive = true, GrantedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpiresAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                GrantedByUserId = adminId, Reason = "Smještaj za akademsku godinu 2025/26"
            },
            new AccessRight
            {
                Id = ar2Id, UserId = studentId, ResourceId = res1Id, AccessType = AccessType.CommonArea,
                IsActive = true, GrantedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                GrantedByUserId = adminId, Reason = "Pristup kuhinji"
            },
            new AccessRight
            {
                Id = ar3Id, UserId = studentId, ResourceId = res4Id, AccessType = AccessType.Network,
                IsActive = true, GrantedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                GrantedByUserId = adminId, Reason = "Pristup WiFi mreži"
            }
        );

        // Requests
        var req1Id = Guid.Parse("11111111-aaaa-bbbb-cccc-111111111111");
        var req2Id = Guid.Parse("22222222-aaaa-bbbb-cccc-222222222222");
        var req3Id = Guid.Parse("33333333-aaaa-bbbb-cccc-333333333333");

        modelBuilder.Entity<Request>().HasData(
            new Request
            {
                Id = req1Id, RequestedByUserId = studentId, RequestType = RequestType.Maintenance,
                Title = "Pokvaren radijator u sobi A-101",
                Description = "Radijator ne grije, molim hitnu intervenciju jer je hladno.",
                Status = RequestStatus.InProgress, Priority = Priority.High,
                AssignedToUserId = staffId, RoomId = room1Id,
                CreatedAt = new DateTime(2025, 11, 15, 10, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 11, 16, 8, 0, 0, DateTimeKind.Utc)
            },
            new Request
            {
                Id = req2Id, RequestedByUserId = studentId, RequestType = RequestType.InventoryReplacement,
                Title = "Zamjena stolice",
                Description = "Stolica u sobi je slomljena, potrebna zamjena.",
                Status = RequestStatus.Pending, Priority = Priority.Medium,
                RoomId = room1Id,
                CreatedAt = new DateTime(2025, 12, 1, 14, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 12, 1, 14, 0, 0, DateTimeKind.Utc)
            },
            new Request
            {
                Id = req3Id, RequestedByUserId = studentId, RequestType = RequestType.ResidenceCertificate,
                Title = "Potvrda o boravku",
                Description = "Trebam potvrdu o boravku za fakultet.",
                Status = RequestStatus.Resolved, Priority = Priority.Low,
                AssignedToUserId = adminId,
                CreatedAt = new DateTime(2025, 10, 5, 9, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 10, 7, 11, 0, 0, DateTimeKind.Utc),
                ResolvedAt = new DateTime(2025, 10, 7, 11, 0, 0, DateTimeKind.Utc)
            }
        );

        // Comments
        modelBuilder.Entity<Comment>().HasData(
            new Comment
            {
                Id = Guid.Parse("aaaaaaaa-1111-2222-3333-444444444444"),
                RequestId = req1Id, AuthorUserId = staffId,
                Content = "Pregled je zakazan za sutra u 10h.",
                IsInternal = false,
                CreatedAt = new DateTime(2025, 11, 16, 8, 0, 0, DateTimeKind.Utc)
            },
            new Comment
            {
                Id = Guid.Parse("bbbbbbbb-1111-2222-3333-444444444444"),
                RequestId = req1Id, AuthorUserId = adminId,
                Content = "Trebamo naručiti novi radijator, stari je neispravan.",
                IsInternal = true,
                CreatedAt = new DateTime(2025, 11, 16, 9, 0, 0, DateTimeKind.Utc)
            },
            new Comment
            {
                Id = Guid.Parse("cccccccc-1111-2222-3333-444444444444"),
                RequestId = req3Id, AuthorUserId = adminId,
                Content = "Potvrda je izdana. Možete je preuzeti u kancelariji.",
                IsInternal = false,
                CreatedAt = new DateTime(2025, 10, 7, 11, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
