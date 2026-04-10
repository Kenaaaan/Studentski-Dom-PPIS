using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentskiDom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ResourceType = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Floor = table.Column<int>(type: "integer", nullable: false),
                    Building = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RoomType = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    StudentStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessRights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccessType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GrantedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessRights_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AccessRights_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AccessRights_Users_GrantedByUserId",
                        column: x => x.GrantedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccessRights_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestType = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    AssignedToUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Requests_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Requests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessRightId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessAction = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessLogs_AccessRights_AccessRightId",
                        column: x => x.AccessRightId,
                        principalTable: "AccessRights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsInternal = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "Description", "IsActive", "Location", "Name", "ResourceType" },
                values: new object[,]
                {
                    { new Guid("e5555555-5555-5555-5555-555555555551"), "Zajednička kuhinja na prvom spratu", true, "Paviljon A, 1. sprat", "Kuhinja - Paviljon A", 1 },
                    { new Guid("e5555555-5555-5555-5555-555555555552"), "Fitness centar za studente", true, "Paviljon B, prizemlje", "Teretana", 2 },
                    { new Guid("e5555555-5555-5555-5555-555555555553"), "Učionica za grupno učenje", true, "Paviljon A, 3. sprat", "Učionica 1", 3 },
                    { new Guid("e5555555-5555-5555-5555-555555555554"), "Pristup internet mreži u domu", true, "Cijeli dom", "WiFi Mreža", 5 }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Building", "Capacity", "Floor", "IsAvailable", "RoomNumber", "RoomType" },
                values: new object[,]
                {
                    { new Guid("d4444444-4444-4444-4444-444444444441"), "Paviljon A", 2, 1, true, "A-101", 1 },
                    { new Guid("d4444444-4444-4444-4444-444444444442"), "Paviljon A", 2, 1, true, "A-102", 1 },
                    { new Guid("d4444444-4444-4444-4444-444444444443"), "Paviljon A", 1, 2, false, "A-201", 0 },
                    { new Guid("d4444444-4444-4444-4444-444444444444"), "Paviljon B", 3, 1, true, "B-101", 2 },
                    { new Guid("d4444444-4444-4444-4444-444444444445"), "Paviljon B", 2, 2, true, "B-201", 1 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "PhoneNumber", "Role", "StudentStatus", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@studentskidom.ba", "Admin", "Adminović", "$2a$11$tIctmXJSmbowkf.lIjr2l.ru4slaMMT7yMXDiwu3kZIoWVVWNPyv.", "+38761000001", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff@studentskidom.ba", "Tehničar", "Tehničarević", "$2a$11$x2gNsrCKHCqawyfD0ZLtOug0Mt718Ot8UCSuc.Shx0JBSS.KKEPLW", "+38761000002", 2, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c3333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student@studentskidom.ba", "Student", "Studentović", "$2a$11$XZatqBSc46WICE9BVgY6YuDTm6q5mkTSK8DNBG.0sKCApqNwReYv.", "+38761000003", 0, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AccessRights",
                columns: new[] { "Id", "AccessType", "ExpiresAt", "GrantedAt", "GrantedByUserId", "IsActive", "Reason", "ResourceId", "RoomId", "UserId" },
                values: new object[,]
                {
                    { new Guid("f6666666-6666-6666-6666-666666666661"), 0, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("a1111111-1111-1111-1111-111111111111"), true, "Smještaj za akademsku godinu 2025/26", null, new Guid("d4444444-4444-4444-4444-444444444441"), new Guid("c3333333-3333-3333-3333-333333333333") },
                    { new Guid("f6666666-6666-6666-6666-666666666662"), 1, null, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("a1111111-1111-1111-1111-111111111111"), true, "Pristup kuhinji", new Guid("e5555555-5555-5555-5555-555555555551"), null, new Guid("c3333333-3333-3333-3333-333333333333") },
                    { new Guid("f6666666-6666-6666-6666-666666666663"), 2, null, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("a1111111-1111-1111-1111-111111111111"), true, "Pristup WiFi mreži", new Guid("e5555555-5555-5555-5555-555555555554"), null, new Guid("c3333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Id", "AssignedToUserId", "CreatedAt", "Description", "Priority", "RequestType", "RequestedByUserId", "ResolvedAt", "RoomId", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-bbbb-cccc-111111111111"), new Guid("b2222222-2222-2222-2222-222222222222"), new DateTime(2025, 11, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Radijator ne grije, molim hitnu intervenciju jer je hladno.", 2, 0, new Guid("c3333333-3333-3333-3333-333333333333"), null, new Guid("d4444444-4444-4444-4444-444444444441"), 1, "Pokvaren radijator u sobi A-101", new DateTime(2025, 11, 16, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-aaaa-bbbb-cccc-222222222222"), null, new DateTime(2025, 12, 1, 14, 0, 0, 0, DateTimeKind.Utc), "Stolica u sobi je slomljena, potrebna zamjena.", 1, 1, new Guid("c3333333-3333-3333-3333-333333333333"), null, new Guid("d4444444-4444-4444-4444-444444444441"), 0, "Zamjena stolice", new DateTime(2025, 12, 1, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-aaaa-bbbb-cccc-333333333333"), new Guid("a1111111-1111-1111-1111-111111111111"), new DateTime(2025, 10, 5, 9, 0, 0, 0, DateTimeKind.Utc), "Trebam potvrdu o boravku za fakultet.", 0, 2, new Guid("c3333333-3333-3333-3333-333333333333"), new DateTime(2025, 10, 7, 11, 0, 0, 0, DateTimeKind.Utc), null, 2, "Potvrda o boravku", new DateTime(2025, 10, 7, 11, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "AuthorUserId", "Content", "CreatedAt", "IsInternal", "RequestId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-2222-3333-444444444444"), new Guid("b2222222-2222-2222-2222-222222222222"), "Pregled je zakazan za sutra u 10h.", new DateTime(2025, 11, 16, 8, 0, 0, 0, DateTimeKind.Utc), false, new Guid("11111111-aaaa-bbbb-cccc-111111111111") },
                    { new Guid("bbbbbbbb-1111-2222-3333-444444444444"), new Guid("a1111111-1111-1111-1111-111111111111"), "Trebamo naručiti novi radijator, stari je neispravan.", new DateTime(2025, 11, 16, 9, 0, 0, 0, DateTimeKind.Utc), true, new Guid("11111111-aaaa-bbbb-cccc-111111111111") },
                    { new Guid("cccccccc-1111-2222-3333-444444444444"), new Guid("a1111111-1111-1111-1111-111111111111"), "Potvrda je izdana. Možete je preuzeti u kancelariji.", new DateTime(2025, 10, 7, 11, 0, 0, 0, DateTimeKind.Utc), false, new Guid("33333333-aaaa-bbbb-cccc-333333333333") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_AccessRightId",
                table: "AccessLogs",
                column: "AccessRightId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_UserId",
                table: "AccessLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRights_GrantedByUserId",
                table: "AccessRights",
                column: "GrantedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRights_ResourceId",
                table: "AccessRights",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRights_RoomId",
                table: "AccessRights",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRights_UserId",
                table: "AccessRights",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorUserId",
                table: "Comments",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RequestId",
                table: "Comments",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_AssignedToUserId",
                table: "Requests",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestedByUserId",
                table: "Requests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RoomId",
                table: "Requests",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomNumber",
                table: "Rooms",
                column: "RoomNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessLogs");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "AccessRights");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
