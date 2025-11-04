using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AnnualRankingAndChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add columns only if they don't exist already
            migrationBuilder.Sql(@"
IF COL_LENGTH('TournamentEnrollments','EnrollmentDate') IS NULL
BEGIN
    ALTER TABLE [TournamentEnrollments]
    ADD [EnrollmentDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000'
END

IF COL_LENGTH('TournamentEnrollments','PlayerId') IS NULL
BEGIN
    ALTER TABLE [TournamentEnrollments]
    ADD [PlayerId] int NOT NULL DEFAULT 0
END
");

            migrationBuilder.CreateTable(
                name: "AnnualTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SuspendedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResumedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    RequesterPlayerId = table.Column<int>(type: "int", nullable: false),
                    RequesterPartnerPlayerId = table.Column<int>(type: "int", nullable: false),
                    TargetPlayerId = table.Column<int>(type: "int", nullable: false),
                    TargetPartnerPlayerId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WinnerPlayerId = table.Column<int>(type: "int", nullable: true),
                    WinnerPartnerPlayerId = table.Column<int>(type: "int", nullable: true),
                    Sets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequesterPointsAtCreation = table.Column<int>(type: "int", nullable: false),
                    TargetPointsAtCreation = table.Column<int>(type: "int", nullable: false),
                    PointsAwardedPerPlayer = table.Column<int>(type: "int", nullable: false),
                    ValidatedByAdminUserId = table.Column<int>(type: "int", nullable: true),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RankingEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnualTableId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PointsTotal = table.Column<int>(type: "int", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Losses = table.Column<int>(type: "int", nullable: false),
                    Draws = table.Column<int>(type: "int", nullable: false),
                    PointsFromTournaments = table.Column<int>(type: "int", nullable: false),
                    PointsFromChallenges = table.Column<int>(type: "int", nullable: false),
                    PointsFromClasses = table.Column<int>(type: "int", nullable: false),
                    PointsFromMatchWins = table.Column<int>(type: "int", nullable: false),
                    PointsFromMatchLosses = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankingEntries_AnnualTables_AnnualTableId",
                        column: x => x.AnnualTableId,
                        principalTable: "AnnualTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoringRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnualTableId = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    BasePoints = table.Column<int>(type: "int", nullable: false),
                    Multiplier = table.Column<float>(type: "real", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoringRules_AnnualTables_AnnualTableId",
                        column: x => x.AnnualTableId,
                        principalTable: "AnnualTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankingEntries_AnnualTableId",
                table: "RankingEntries",
                column: "AnnualTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoringRules_AnnualTableId",
                table: "ScoringRules",
                column: "AnnualTableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RankingEntries");

            migrationBuilder.DropTable(
                name: "ScoringRules");

            migrationBuilder.DropTable(
                name: "AnnualTables");

            // Optional: keep columns if pre-existing; no-op on down to avoid dropping existing schema
        }
    }
}
