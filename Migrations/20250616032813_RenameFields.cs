using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class RenameFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Person",
                newName: "Birthdate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Birthdate",
                table: "Person",
                newName: "BirthDate");
        }
    }
}
