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
            // Insert only if the permission doesn't already exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [PermissionComponents] WHERE [Id] = 50)
                BEGIN
                    SET IDENTITY_INSERT [PermissionComponents] ON;
                    INSERT INTO [PermissionComponents] ([Id], [Description], [DisplayName], [ModuleId], [Name], [PermissionType])
                    VALUES (50, N'Permite eliminar torneos sin inscripciones', N'Eliminar torneo', 5, N'tournament:delete', N'Simple');
                    SET IDENTITY_INSERT [PermissionComponents] OFF;
                END
            ");
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
