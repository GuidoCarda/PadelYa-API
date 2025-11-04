using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingModuleAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Milestones",
                table: "Stats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                table: "Stats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAt",
                table: "Stats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RecordedByTeacherId",
                table: "Stats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[] { 9, "Ranking", "ranking" });

            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType" },
                values: new object[,]
                {
                    { 50, "Permite ver el ranking propio y crear desafíos", "Ver ranking propio", 9, "ranking:view_own", "Simple" },
                    { 51, "Permite ver el ranking completo", "Ver ranking", 9, "ranking:view", "Simple" },
                    { 52, "Permite gestionar la tabla anual y validar desafíos", "Gestionar ranking", 9, "ranking:manage", "Simple" }
                });

            migrationBuilder.InsertData(
                table: "RolCompositePermission",
                columns: new[] { "PermissionComponentId", "RoleId" },
                values: new object[,]
                {
                    { 50, 100 },
                    { 50, 102 },
                    { 51, 100 },
                    { 52, 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_RecordedByTeacherId",
                table: "Stats",
                column: "RecordedByTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stats_Person_RecordedByTeacherId",
                table: "Stats",
                column: "RecordedByTeacherId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stats_Person_RecordedByTeacherId",
                table: "Stats");

            migrationBuilder.DropIndex(
                name: "IX_Stats_RecordedByTeacherId",
                table: "Stats");

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 50, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 50, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 51, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 52, 100 });

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "Milestones",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Observations",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "RecordedByTeacherId",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Exercises");
        }
    }
}
