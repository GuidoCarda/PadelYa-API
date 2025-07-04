using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class UseTimeOnlyForCourtAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "CourtAvailabilities",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "CourtAvailabilities",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(23, 0, 0), new TimeOnly(6, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(22, 0, 0), new TimeOnly(7, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CourtStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CourtStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CourtStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 4,
                column: "CourtStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 5,
                column: "CourtStatus",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "CourtAvailabilities",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "CourtAvailabilities",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "EndTime", "StartTime" },
                values: new object[] { new DateTime(2024, 1, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CourtStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CourtStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CourtStatus",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 4,
                column: "CourtStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 5,
                column: "CourtStatus",
                value: 0);
        }
    }
}
