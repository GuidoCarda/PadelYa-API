using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "Birthdate", "Category", "Institution", "PersonType", "Title" },
                values: new object[] { 4, new DateTime(1985, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Profesional", "PadelYa Academy", "Teacher", "Profesor Certificado" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Name", "PasswordHash", "PersonId", "RoleId", "Surname" },
                values: new object[] { "admin@padelya.com", "Admin", "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==", null, 100, "System" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name", "PasswordHash", "PersonId", "RoleId", "Surname" },
                values: new object[] { "teacher@padelya.com", "María", "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==", 4, 101, "González" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name", "PasswordHash", "PersonId", "Surname" },
                values: new object[] { "player1@padelya.com", "Juan", "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==", 1, "Pérez" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "PersonId", "RefreshToken", "RefreshTokenExpiryTime", "RoleId", "StatusId", "Surname" },
                values: new object[,]
                {
                    { 4, "player2@padelya.com", "Ana", "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==", 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, 1, "García" },
                    { 5, "player3@padelya.com", "Luis", "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==", 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, 1, "Martínez" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Name", "PasswordHash", "PersonId", "RoleId", "Surname" },
                values: new object[] { "user1@test.com", "Juan", "test", 1, 102, "Pérez" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name", "PasswordHash", "PersonId", "RoleId", "Surname" },
                values: new object[] { "user2@test.com", "Ana", "test", 2, 102, "García" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name", "PasswordHash", "PersonId", "Surname" },
                values: new object[] { "user3@test.com", "Luis", "test", 3, "Martínez" });
        }
    }
}
