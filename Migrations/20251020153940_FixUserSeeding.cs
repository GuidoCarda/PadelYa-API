using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class FixUserSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "player1@padelya.com", "Juan", "+598 91 234 567", "Pérez" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "player2@padelya.com", "Ana", "+598 92 345 678", "García" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "player3@padelya.com", "Luis", "+598 93 456 789", "Martínez" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "teacher@padelya.com", "María", "+598 94 567 890", "González" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "", "", "", "" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "", "", "", "" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "", "", "", "" });

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "Name", "PhoneNumber", "Surname" },
                values: new object[] { "", "", "", "" });
        }
    }
}
