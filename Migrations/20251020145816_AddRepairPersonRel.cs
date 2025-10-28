using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRepairPersonRel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Repairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_PersonId",
                table: "Repairs",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repairs_Person_PersonId",
                table: "Repairs",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repairs_Person_PersonId",
                table: "Repairs");

            migrationBuilder.DropIndex(
                name: "IX_Repairs_PersonId",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Repairs");
        }
    }
}
