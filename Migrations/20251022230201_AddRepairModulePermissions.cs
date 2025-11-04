using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
  /// <inheritdoc />
  public partial class AddRepairModulePermissions : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.InsertData(
          table: "Modules",
          columns: new[] { "Id", "DisplayName", "Name" },
          values: new object[] { 9, "Reparaciones", "repair" });

      migrationBuilder.InsertData(
          table: "PermissionComponents",
          columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType" },
          values: new object[,]
          {
                    { 55, "Permite crear nuevas reparaciones", "Crear reparación", 9, "repair:create", "Simple" },
                    { 56, "Permite editar reparaciones", "Editar reparación", 9, "repair:edit", "Simple" },
                    { 57, "Permite cancelar reparaciones", "Cancelar reparación", 9, "repair:cancel", "Simple" },
                    { 58, "Permite ver reparaciones", "Ver reparaciones", 9, "repair:view", "Simple" },
                    { 59, "Permite ver la reparación del usuario", "Ver reparación propia", 9, "repair:view_own", "Simple" }
          });

      migrationBuilder.InsertData(
          table: "RolCompositePermission",
          columns: new[] { "PermissionComponentId", "RoleId" },
          values: new object[,]
          {
                    { 55, 100 },
                    { 56, 100 },
                    { 57, 100 },
                    { 58, 100 },
                    { 59, 100 }
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 55, 100 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 56, 100 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 57, 100 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 58, 100 });

      migrationBuilder.DeleteData(
          table: "RolCompositePermission",
          keyColumns: new[] { "PermissionComponentId", "RoleId" },
          keyValues: new object[] { 59, 100 });

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 55);

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 56);

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 57);

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 58);

      migrationBuilder.DeleteData(
          table: "PermissionComponents",
          keyColumn: "Id",
          keyValue: 59);

      migrationBuilder.DeleteData(
          table: "Modules",
          keyColumn: "Id",
          keyValue: 9);
    }
  }
}
