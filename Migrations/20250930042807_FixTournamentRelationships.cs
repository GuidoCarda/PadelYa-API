using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class FixTournamentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentBrackets_TournamentPhases_PhaseId",
                table: "TournamentBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentEnrollments_Couples_CoupleId",
                table: "TournamentEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentBrackets_BracketId",
                table: "TournamentMatches");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "TournamentPhases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PhaseName",
                table: "TournamentPhases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TournamentPhases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentBrackets_TournamentPhases_PhaseId",
                table: "TournamentBrackets",
                column: "PhaseId",
                principalTable: "TournamentPhases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentEnrollments_Couples_CoupleId",
                table: "TournamentEnrollments",
                column: "CoupleId",
                principalTable: "Couples",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_TournamentBrackets_BracketId",
                table: "TournamentMatches",
                column: "BracketId",
                principalTable: "TournamentBrackets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentBrackets_TournamentPhases_PhaseId",
                table: "TournamentBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentEnrollments_Couples_CoupleId",
                table: "TournamentEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentBrackets_BracketId",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TournamentPhases");

            migrationBuilder.DropColumn(
                name: "PhaseName",
                table: "TournamentPhases");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TournamentPhases");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentBrackets_TournamentPhases_PhaseId",
                table: "TournamentBrackets",
                column: "PhaseId",
                principalTable: "TournamentPhases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentEnrollments_Couples_CoupleId",
                table: "TournamentEnrollments",
                column: "CoupleId",
                principalTable: "Couples",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_TournamentBrackets_BracketId",
                table: "TournamentMatches",
                column: "BracketId",
                principalTable: "TournamentBrackets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
