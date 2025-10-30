using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentDeletePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PermissionComponents",
                columns: new[] { "Id", "Description", "DisplayName", "ModuleId", "Name", "PermissionType" },
                values: new object[] { 50, "Permite eliminar torneos sin inscripciones", "Eliminar torneo", 5, "tournament:delete", "Simple" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 50);
        }
    }
}
