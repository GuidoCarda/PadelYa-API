using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentPermissionsToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RolCompositePermission",
                columns: new[] { "PermissionComponentId", "RoleId" },
                values: new object[,]
                {
                    { 13, 102 },
                    { 14, 102 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 13, 102 });

            migrationBuilder.DeleteData(
                table: "RolCompositePermission",
                keyColumns: new[] { "PermissionComponentId", "RoleId" },
                keyValues: new object[] { 14, 102 });
        }
    }
}
