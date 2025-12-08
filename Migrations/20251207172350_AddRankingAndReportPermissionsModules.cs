using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingAndReportPermissionsModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisteredAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            // Module 10 (Ranking) already exists from migration AddRankingModuleAndPermissions
            // Only insert Module 11 (Reportes)
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[] { 11, "Reportes", "report" });

            // Permissions 60, 61, 62 already exist from migration AddRankingModuleAndPermissions
            // Only insert Permission 63 (report:view)
            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType", "RequiredEntity" },
                values: new object[] { 63, "Permite ver reportes", "Ver reportes", 11, "report:view", "Simple", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Only delete what this migration inserted
            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisteredAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");
        }
    }
}
