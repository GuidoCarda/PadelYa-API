using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddCourtSlotStatusAndBookingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CourtSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CancellationReason", "CancelledAt", "CancelledBy", "Status" },
                values: new object[] { null, null, null, "Reserved_Paid" });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CancellationReason", "CancelledAt", "CancelledBy", "Status" },
                values: new object[] { null, null, null, "Reserved_Deposit" });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CancellationReason", "CancelledAt", "CancelledBy", "Status" },
                values: new object[] { null, null, null, "Reserved_Paid" });

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CancellationReason", "CancelledAt", "CancelledBy", "Status" },
                values: new object[] { null, null, null, "Reserved_Paid" });

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 4,
                column: "Status",
                value: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CourtSlots");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "Bookings");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "reserved_paid");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "reserved_deposit");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "reserved_paid");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 4,
                column: "Status",
                value: "reserved_paid");
        }
    }
}
