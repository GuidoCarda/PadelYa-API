using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class SlotHoldFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "CourtSlots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalReference",
                table: "CourtSlots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferenceId",
                table: "CourtSlots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ExpiresAt", "ExternalReference", "PreferenceId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExpiresAt", "ExternalReference", "PreferenceId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ExpiresAt", "ExternalReference", "PreferenceId" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ExpiresAt", "ExternalReference", "PreferenceId" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "CourtSlots");

            migrationBuilder.DropColumn(
                name: "ExternalReference",
                table: "CourtSlots");

            migrationBuilder.DropColumn(
                name: "PreferenceId",
                table: "CourtSlots");
        }
    }
}
