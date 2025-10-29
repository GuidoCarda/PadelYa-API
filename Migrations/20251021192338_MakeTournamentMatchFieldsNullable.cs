using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class MakeTournamentMatchFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_CourtSlots_CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_TournamentMatches_CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.AlterColumn<int>(
                name: "CourtSlotId",
                table: "TournamentMatches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CoupleTwoId",
                table: "TournamentMatches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CoupleOneId",
                table: "TournamentMatches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_CourtSlotId",
                table: "TournamentMatches",
                column: "CourtSlotId",
                unique: true,
                filter: "[CourtSlotId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_CourtSlots_CourtSlotId",
                table: "TournamentMatches",
                column: "CourtSlotId",
                principalTable: "CourtSlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_CourtSlots_CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_TournamentMatches_CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.AlterColumn<int>(
                name: "CourtSlotId",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CoupleTwoId",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CoupleOneId",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_CourtSlotId",
                table: "TournamentMatches",
                column: "CourtSlotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_CourtSlots_CourtSlotId",
                table: "TournamentMatches",
                column: "CourtSlotId",
                principalTable: "CourtSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
