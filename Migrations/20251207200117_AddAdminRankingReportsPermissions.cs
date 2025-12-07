using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRankingReportsPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 60,
                column: "ModuleId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 61,
                column: "ModuleId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 62,
                column: "ModuleId",
                value: 10);

            // All role-permission relationships (60, 100), (62, 100), (63, 100) already exist
            // in the database from previous migrations or partial runs - no inserts needed
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No RolCompositePermission changes needed - data was pre-existing

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 60,
                column: "ModuleId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 61,
                column: "ModuleId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 62,
                column: "ModuleId",
                value: 11);
        }
    }
}
