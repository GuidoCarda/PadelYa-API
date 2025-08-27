using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissionDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite a un cliente reservar un turno en su propia cuenta", "Hacer reserva" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermissionComponents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DisplayName" },
                values: new object[] { "Permite al jugador crear su propia reserva", "Crear reserva (cliente)" });
        }
    }
}
