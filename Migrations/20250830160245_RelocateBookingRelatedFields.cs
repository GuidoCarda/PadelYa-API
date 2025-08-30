using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class RelocateBookingRelatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalReference",
                table: "CourtSlots");

            migrationBuilder.DropColumn(
                name: "PreferenceId",
                table: "CourtSlots");

            migrationBuilder.AddColumn<string>(
                name: "PreferenceId",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "PreferenceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "PreferenceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3,
                column: "PreferenceId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 4,
                column: "PreferenceId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferenceId",
                table: "Bookings");

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
                columns: new[] { "ExternalReference", "PreferenceId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExternalReference", "PreferenceId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ExternalReference", "PreferenceId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ExternalReference", "PreferenceId" },
                values: new object[] { null, null });
        }
    }
}
