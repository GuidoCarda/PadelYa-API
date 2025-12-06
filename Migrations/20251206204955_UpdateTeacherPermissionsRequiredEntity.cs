using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherPermissionsRequiredEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 22,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 23,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 24,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 25,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 27,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 41,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 42,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 43,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 44,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 45,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 46,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 47,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 48,
                column: "RequiredEntity",
                value: "Teacher");

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 49,
                column: "RequiredEntity",
                value: "Teacher");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 27,
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
        }
    }
}
