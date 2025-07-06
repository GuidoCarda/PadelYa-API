using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Person_PlayerId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Person_PersonId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PersonId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_PlayerId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Lessons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PersonId",
                table: "Payments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_PlayerId",
                table: "Lessons",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Person_PlayerId",
                table: "Lessons",
                column: "PlayerId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Person_PersonId",
                table: "Payments",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
