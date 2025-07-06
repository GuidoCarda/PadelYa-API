using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCourtAvailabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourtAvailabilities");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ClosingTime",
                table: "Courts",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpeningTime",
                table: "Courts",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ClosingTime", "OpeningTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Courts");

            migrationBuilder.DropColumn(
                name: "OpeningTime",
                table: "Courts");

            migrationBuilder.CreateTable(
                name: "CourtAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtAvailabilities_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CourtAvailabilities",
                columns: new[] { "Id", "CourtId", "EndTime", "StartTime", "Weekday" },
                values: new object[,]
                {
                    { 1, 1, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 1 },
                    { 2, 1, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 2 },
                    { 3, 1, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 3 },
                    { 4, 1, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 4 },
                    { 5, 1, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 5 },
                    { 6, 1, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 6 },
                    { 7, 1, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 0 },
                    { 8, 2, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 1 },
                    { 9, 2, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 2 },
                    { 10, 2, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 3 },
                    { 11, 2, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 4 },
                    { 12, 2, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 5 },
                    { 13, 2, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 6 },
                    { 14, 2, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 0 },
                    { 15, 3, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 1 },
                    { 16, 3, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 2 },
                    { 17, 3, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 3 },
                    { 18, 3, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 4 },
                    { 19, 3, new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0), 5 },
                    { 20, 3, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 6 },
                    { 21, 3, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 0 },
                    { 22, 5, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 1 },
                    { 23, 5, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 2 },
                    { 24, 5, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 3 },
                    { 25, 5, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 4 },
                    { 26, 5, new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0), 5 },
                    { 27, 5, new TimeOnly(21, 0, 0), new TimeOnly(8, 0, 0), 6 },
                    { 28, 5, new TimeOnly(21, 0, 0), new TimeOnly(8, 0, 0), 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourtAvailabilities_CourtId",
                table: "CourtAvailabilities",
                column: "CourtId");
        }
    }
}
