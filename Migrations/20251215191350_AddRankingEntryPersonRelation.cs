using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingEntryPersonRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RankingEntries_PlayerId",
                table: "RankingEntries",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RankingEntries_Persons_PlayerId",
                table: "RankingEntries",
                column: "PlayerId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RankingEntries_Persons_PlayerId",
                table: "RankingEntries");

            migrationBuilder.DropIndex(
                name: "IX_RankingEntries_PlayerId",
                table: "RankingEntries");
        }
    }
}
