using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourtSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 4,
                column: "CourtId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BookingPrice", "Name" },
                values: new object[] { 20, "Cancha 1 - Premium" });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BookingPrice", "Name" },
                values: new object[] { 10, "Cancha 2 - Standard" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CourtSlots",
                keyColumn: "Id",
                keyValue: 4,
                column: "CourtId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BookingPrice", "Name" },
                values: new object[] { 15000, "Court 1 - Premium" });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BookingPrice", "Name" },
                values: new object[] { 20000, "Court 2 - Standard" });

            migrationBuilder.InsertData(
                table: "Courts",
                columns: new[] { "Id", "BookingPrice", "ClosingTime", "ComplexId", "CourtStatus", "Name", "OpeningTime", "Type" },
                values: new object[,]
                {
                    { 3, 12000, new TimeOnly(23, 0, 0), 1, 1, "Court 3 - Standard", new TimeOnly(6, 0, 0), "Cristal" },
                    { 4, 25000, new TimeOnly(23, 0, 0), 1, 0, "Court 4 - Premium", new TimeOnly(6, 0, 0), "Césped" },
                    { 5, 20000, new TimeOnly(23, 0, 0), 1, 1, "Court 5 - Indoor", new TimeOnly(6, 0, 0), "Cristal" }
                });
        }
    }
}
