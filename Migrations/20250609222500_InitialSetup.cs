using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionComponents_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolCompositePermission",
                columns: table => new
                {
                    PermissionComponentId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolCompositePermission", x => new { x.PermissionComponentId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RolCompositePermission_PermissionComponents_PermissionComponentId",
                        column: x => x.PermissionComponentId,
                        principalTable: "PermissionComponents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolCompositePermission_PermissionComponents_RoleId",
                        column: x => x.RoleId,
                        principalTable: "PermissionComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_PermissionComponents_RoleId",
                        column: x => x.RoleId,
                        principalTable: "PermissionComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "UserStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreferredPosition = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Booking" },
                    { 2, "CourtManagement" },
                    { 3, "UserManagement" },
                    { 4, "RoleManagement" },
                    { 5, "Tournaments" },
                    { 6, "Classes" }
                });

            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "Name", "PermissionType" },
                values: new object[,]
                {
                    { 100, "Admin", "Composite" },
                    { 101, "Teacher", "Composite" },
                    { 102, "Player", "Composite" }
                });

            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "FormId", "Name", "PermissionType" },
                values: new object[,]
                {
                    { 1, 1, "Booking_View", "Simple" },
                    { 2, 1, "Booking_Create", "Simple" },
                    { 3, 2, "CourtManagement_View", "Simple" },
                    { 4, 3, "UserManagement_View", "Simple" },
                    { 5, 4, "RoleManagement_View", "Simple" },
                    { 6, 5, "Tournaments_View", "Simple" },
                    { 7, 6, "Classes_View", "Simple" }
                });

            migrationBuilder.InsertData(
                table: "RolCompositePermission",
                columns: new[] { "PermissionComponentId", "RoleId" },
                values: new object[,]
                {
                    { 1, 100 },
                    { 1, 101 },
                    { 1, 102 },
                    { 2, 100 },
                    { 2, 102 },
                    { 3, 100 },
                    { 4, 100 },
                    { 5, 100 },
                    { 6, 100 },
                    { 6, 101 },
                    { 7, 100 },
                    { 7, 101 },
                    { 7, 102 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionComponents_FormId",
                table: "PermissionComponents",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolCompositePermission_RoleId",
                table: "RolCompositePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatusId",
                table: "Users",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "RolCompositePermission");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PermissionComponents");

            migrationBuilder.DropTable(
                name: "UserStatuses");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
