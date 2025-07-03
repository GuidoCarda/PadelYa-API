using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddComplexSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Court_Complex_ComplexId",
                table: "Court");

            migrationBuilder.DropForeignKey(
                name: "FK_CourtAvailability_Court_CourtId",
                table: "CourtAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourtAvailability",
                table: "CourtAvailability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Court",
                table: "Court");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Court");

            migrationBuilder.RenameTable(
                name: "CourtAvailability",
                newName: "CourtAvailabilities");

            migrationBuilder.RenameTable(
                name: "Court",
                newName: "Courts");

            migrationBuilder.RenameIndex(
                name: "IX_CourtAvailability_CourtId",
                table: "CourtAvailabilities",
                newName: "IX_CourtAvailabilities_CourtId");

            migrationBuilder.RenameIndex(
                name: "IX_Court_ComplexId",
                table: "Courts",
                newName: "IX_Courts_ComplexId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Complex",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourtAvailabilities",
                table: "CourtAvailabilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courts",
                table: "Courts",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Complex",
                columns: new[] { "Id", "Address", "ClosingTime", "Name", "OpeningTime" },
                values: new object[] { 1, "123 Sports Avenue, Downtown District, City Center", new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), "PadelYa Sports Complex", new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Courts",
                columns: new[] { "Id", "BookingPrice", "ComplexId", "CourtStatus", "Name" },
                values: new object[,]
                {
                    { 1, 15000, 1, 0, "Court 1 - Premium" },
                    { 2, 20000, 1, 0, "Court 2 - Standard" },
                    { 3, 12000, 1, 0, "Court 3 - Standard" },
                    { 4, 25000, 1, 1, "Court 4 - Premium" },
                    { 5, 20000, 1, 0, "Court 5 - Indoor" }
                });

            migrationBuilder.InsertData(
                table: "CourtAvailabilities",
                columns: new[] { "Id", "CourtId", "EndTime", "StartTime", "Weekday" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 1, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 1, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, 1, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 5, 1, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 6, 1, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 7, 1, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 8, 2, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 9, 2, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 10, 2, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 11, 2, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 12, 2, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 13, 2, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 14, 2, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 15, 3, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 16, 3, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 17, 3, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 18, 3, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 19, 3, new DateTime(2024, 1, 1, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 6, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 20, 3, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 21, 3, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 22, 5, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 23, 5, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 24, 5, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 25, 5, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 26, 5, new DateTime(2024, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 7, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 27, 5, new DateTime(2024, 1, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 28, 5, new DateTime(2024, 1, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAvailabilities_Courts_CourtId",
                table: "CourtAvailabilities",
                column: "CourtId",
                principalTable: "Courts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courts_Complex_ComplexId",
                table: "Courts",
                column: "ComplexId",
                principalTable: "Complex",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourtAvailabilities_Courts_CourtId",
                table: "CourtAvailabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Courts_Complex_ComplexId",
                table: "Courts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courts",
                table: "Courts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourtAvailabilities",
                table: "CourtAvailabilities");

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "CourtAvailabilities",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Complex",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Complex");

            migrationBuilder.RenameTable(
                name: "Courts",
                newName: "Court");

            migrationBuilder.RenameTable(
                name: "CourtAvailabilities",
                newName: "CourtAvailability");

            migrationBuilder.RenameIndex(
                name: "IX_Courts_ComplexId",
                table: "Court",
                newName: "IX_Court_ComplexId");

            migrationBuilder.RenameIndex(
                name: "IX_CourtAvailabilities_CourtId",
                table: "CourtAvailability",
                newName: "IX_CourtAvailability_CourtId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Court",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Court",
                table: "Court",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourtAvailability",
                table: "CourtAvailability",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Court_Complex_ComplexId",
                table: "Court",
                column: "ComplexId",
                principalTable: "Complex",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourtAvailability_Court_CourtId",
                table: "CourtAvailability",
                column: "CourtId",
                principalTable: "Court",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
