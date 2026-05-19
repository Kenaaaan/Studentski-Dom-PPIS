using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentskiDom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnavailableUntilToResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UnavailableUntil",
                table: "Resources",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "Requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("11111111-aaaa-bbbb-cccc-111111111111"),
                column: "ResourceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("22222222-aaaa-bbbb-cccc-222222222222"),
                column: "ResourceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Requests",
                keyColumn: "Id",
                keyValue: new Guid("33333333-aaaa-bbbb-cccc-333333333333"),
                column: "ResourceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("e5555555-5555-5555-5555-555555555551"),
                column: "UnavailableUntil",
                value: null);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("e5555555-5555-5555-5555-555555555552"),
                column: "UnavailableUntil",
                value: null);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("e5555555-5555-5555-5555-555555555553"),
                column: "UnavailableUntil",
                value: null);

            migrationBuilder.UpdateData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: new Guid("e5555555-5555-5555-5555-555555555554"),
                column: "UnavailableUntil",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$7352M.gj45i0ZcnBR5ck4uS.pjpI5qbKpGprqrkOYA222H.E9iQma");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$/uFnDqLt3ylgvWirEqchPuUUoIYwzTvcnjofCmLgjyH8/Ytwexmwq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "$2a$11$FLje/3HeNVLcbwTb3Urt.OW0LOApw2OSgZ/mGxoQudyp1FalfAhky");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ResourceId",
                table: "Requests",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Resources_ResourceId",
                table: "Requests",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Resources_ResourceId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ResourceId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "UnavailableUntil",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Requests");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$tIctmXJSmbowkf.lIjr2l.ru4slaMMT7yMXDiwu3kZIoWVVWNPyv.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                column: "PasswordHash",
                value: "$2a$11$x2gNsrCKHCqawyfD0ZLtOug0Mt718Ot8UCSuc.Shx0JBSS.KKEPLW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3333333-3333-3333-3333-333333333333"),
                column: "PasswordHash",
                value: "$2a$11$XZatqBSc46WICE9BVgY6YuDTm6q5mkTSK8DNBG.0sKCApqNwReYv.");
        }
    }
}
