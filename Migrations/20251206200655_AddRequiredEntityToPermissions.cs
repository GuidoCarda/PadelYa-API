using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiredEntityToPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredEntity",
                table: "PermissionComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                table: "PermissionComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 1,
                column: "RequiredEntity",
                value: "Player");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 2,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 3,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 4,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 5,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 6,
                column: "RequiredEntity",
                value: "Player");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 7,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 8,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 9,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 10,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 11,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 12,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 13,
                column: "RequiredEntity",
                value: "Player");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 14,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 15,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 16,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 17,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 18,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 19,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 20,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 21,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 22,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 23,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 24,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 25,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 26,
                column: "RequiredEntity",
                value: "Player");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 27,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 28,
                column: "RequiredEntity",
                value: "Player");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 29,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 30,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 31,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 32,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 33,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 34,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 35,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 36,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 37,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 38,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 39,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 40,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 41,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 42,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 43,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 44,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 45,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 46,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 47,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 48,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 49,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 50,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 55,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 56,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 57,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 58,
                column: "RequiredEntity",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 59,
                column: "RequiredEntity",
                value: "Player");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 100,
                column: "TargetType",
                value: null);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 101,
                column: "TargetType",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 102,
                column: "TargetType",
                value: "Player");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredEntity",
                table: "PermissionComponents");

            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "PermissionComponents");
        }
    }
}
