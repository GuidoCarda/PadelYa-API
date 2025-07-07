using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompleteDataWithCorrectOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CourtSlots",
                columns: new[] { "Id", "CourtId", "Date", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(10, 30, 0), new TimeOnly(9, 0, 0) },
                    { 2, 1, new DateTime(2025, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(15, 0, 0), new TimeOnly(13, 30, 0) },
                    { 3, 2, new DateTime(2025, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(12, 0, 0), new TimeOnly(10, 30, 0) },
                    { 4, 3, new DateTime(2025, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeOnly(19, 30, 0), new TimeOnly(18, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "Birthdate", "Category", "PersonType", "PreferredPosition" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Primera", "Player", "Derecha" },
                    { 2, new DateTime(1992, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Segunda", "Player", "Revés" },
                    { 3, new DateTime(1994, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tercera", "Player", "Derecha" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "CourtSlotId", "PersonId", "Status" },
                values: new object[,]
                {
                    { 1, 1, 1, "reserved_paid" },
                    { 2, 2, 2, "reserved_deposit" },
                    { 3, 3, 1, "reserved_paid" },
                    { 4, 4, 3, "reserved_paid" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "PersonId", "RefreshToken", "RefreshTokenExpiryTime", "RoleId", "StatusId", "Surname" },
                values: new object[,]
                {
                    { 1, "user1@test.com", "Juan", "test", 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, 1, "Pérez" },
                    { 2, "user2@test.com", "Ana", "test", 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, 1, "García" },
                    { 3, "user3@test.com", "Luis", "test", 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 102, 1, "Martínez" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
