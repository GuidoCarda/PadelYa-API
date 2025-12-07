using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPhaseOrderToTournamentPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhaseOrder",
                table: "TournamentPhases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinnerCoupleId",
                table: "TournamentMatches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_WinnerCoupleId",
                table: "TournamentMatches",
                column: "WinnerCoupleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_Couples_WinnerCoupleId",
                table: "TournamentMatches",
                column: "WinnerCoupleId",
                principalTable: "Couples",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_Couples_WinnerCoupleId",
                table: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_TournamentMatches_WinnerCoupleId",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "PhaseOrder",
                table: "TournamentPhases");

            migrationBuilder.DropColumn(
                name: "WinnerCoupleId",
                table: "TournamentMatches");
        }
    }
}
