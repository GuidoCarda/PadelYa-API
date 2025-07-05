using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class Tournaments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Couples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Couples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentPhase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TournamentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quota = table.Column<int>(type: "int", nullable: false),
                    EnrollmentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnrollmentStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnrollmentEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TournamentStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TournamentEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CouplePlayer",
                columns: table => new
                {
                    CoupleId = table.Column<int>(type: "int", nullable: false),
                    PlayersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouplePlayer", x => new { x.CoupleId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_CouplePlayer_Couples_CoupleId",
                        column: x => x.CoupleId,
                        principalTable: "Couples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouplePlayer_Person_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoupleId = table.Column<int>(type: "int", nullable: false),
                    TournamentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentEnrollments_Couples_CoupleId",
                        column: x => x.CoupleId,
                        principalTable: "Couples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TournamentEnrollments_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentPhases_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentBracketetse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentBracketetse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentBracketetse_TournamentPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "TournamentPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentMatchState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoupleOneId = table.Column<int>(type: "int", nullable: false),
                    CoupleTwoId = table.Column<int>(type: "int", nullable: false),
                    BracketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_Couples_CoupleOneId",
                        column: x => x.CoupleOneId,
                        principalTable: "Couples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_Couples_CoupleTwoId",
                        column: x => x.CoupleTwoId,
                        principalTable: "Couples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentMatches_TournamentBracketetse_BracketId",
                        column: x => x.BracketId,
                        principalTable: "TournamentBracketetse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouplePlayer_PlayersId",
                table: "CouplePlayer",
                column: "PlayersId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentBracketetse_PhaseId",
                table: "TournamentBracketetse",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentEnrollments_CoupleId",
                table: "TournamentEnrollments",
                column: "CoupleId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentEnrollments_TournamentId",
                table: "TournamentEnrollments",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_BracketId",
                table: "TournamentMatches",
                column: "BracketId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_CoupleOneId",
                table: "TournamentMatches",
                column: "CoupleOneId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_CoupleTwoId",
                table: "TournamentMatches",
                column: "CoupleTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPhases_TournamentId",
                table: "TournamentPhases",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouplePlayer");

            migrationBuilder.DropTable(
                name: "TournamentEnrollments");

            migrationBuilder.DropTable(
                name: "TournamentMatches");

            migrationBuilder.DropTable(
                name: "Couples");

            migrationBuilder.DropTable(
                name: "TournamentBracketetse");

            migrationBuilder.DropTable(
                name: "TournamentPhases");

            migrationBuilder.DropTable(
                name: "Tournaments");
        }
    }
}
