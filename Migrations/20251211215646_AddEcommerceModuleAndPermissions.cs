using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEcommerceModuleAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[] { 12, "Tienda", "ecommerce" });

            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType", "RequiredEntity" },
                values: new object[,]
                {
                    { 65, "Permite crear nuevos productos en el catálogo", "Crear producto", 12, "product:create", "Simple", null },
                    { 66, "Permite editar productos existentes", "Editar producto", 12, "product:edit", "Simple", null },
                    { 67, "Permite eliminar productos del catálogo", "Eliminar producto", 12, "product:delete", "Simple", null },
                    { 68, "Permite ver todos los productos", "Ver productos", 12, "product:view", "Simple", null },
                    { 69, "Permite crear nuevas categorías", "Crear categoría", 12, "category:create", "Simple", null },
                    { 70, "Permite editar categorías existentes", "Editar categoría", 12, "category:edit", "Simple", null },
                    { 71, "Permite eliminar categorías", "Eliminar categoría", 12, "category:delete", "Simple", null },
                    { 72, "Permite ver todas las categorías", "Ver categorías", 12, "category:view", "Simple", null }
                });

            migrationBuilder.InsertData(
                table: "RolCompositePermission",
                columns: new[] { "PermissionComponentId", "RoleId" },
                values: new object[,]
                {
                    { 65, 100 },
                    { 66, 100 },
                    { 67, 100 },
                    { 68, 100 },
                    { 69, 100 },
                    { 70, 100 },
                    { 71, 100 },
                    { 72, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 65, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 66, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 67, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 68, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 69, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 70, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 71, 100 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 72, 100 });

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
