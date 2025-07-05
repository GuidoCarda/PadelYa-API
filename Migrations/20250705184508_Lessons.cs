using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class Lessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentBracketetse_TournamentPhases_PhaseId",
                table: "TournamentBracketetse");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentBracketetse_BracketId",
                table: "TournamentMatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentBracketetse",
                table: "TournamentBracketetse");

            migrationBuilder.RenameTable(
                name: "TournamentBracketetse",
                newName: "TournamentBracketets");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentBracketetse_PhaseId",
                table: "TournamentBracketets",
                newName: "IX_TournamentBracketets_PhaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentBracketets",
                table: "TournamentBracketets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Person_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<TimeOnly>(type: "time", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routines_Person_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonPlayer",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlayer", x => new { x.LessonId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_LessonPlayer_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonPlayer_Person_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Drive = table.Column<float>(type: "real", nullable: false),
                    Backhand = table.Column<float>(type: "real", nullable: false),
                    Smash = table.Column<float>(type: "real", nullable: false),
                    Serve = table.Column<float>(type: "real", nullable: false),
                    Vibora = table.Column<float>(type: "real", nullable: false),
                    Bandeja = table.Column<float>(type: "real", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stats_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stats_Person_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutineExercise",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    RoutineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutineExercise", x => new { x.ExerciseId, x.RoutineId });
                    table.ForeignKey(
                        name: "FK_RoutineExercise_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutineExercise_Routines_RoutineId",
                        column: x => x.RoutineId,
                        principalTable: "Routines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoutinePlayer",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RoutineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutinePlayer", x => new { x.PlayerId, x.RoutineId });
                    table.ForeignKey(
                        name: "FK_RoutinePlayer_Person_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutinePlayer_Routines_RoutineId",
                        column: x => x.RoutineId,
                        principalTable: "Routines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlayer_PlayerId",
                table: "LessonPlayer",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TeacherId",
                table: "Lessons",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutineExercise_RoutineId",
                table: "RoutineExercise",
                column: "RoutineId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutinePlayer_RoutineId",
                table: "RoutinePlayer",
                column: "RoutineId");

            migrationBuilder.CreateIndex(
                name: "IX_Routines_CreatorId",
                table: "Routines",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_LessonId",
                table: "Stats",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_PlayerId",
                table: "Stats",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentBracketets_TournamentPhases_PhaseId",
                table: "TournamentBracketets",
                column: "PhaseId",
                principalTable: "TournamentPhases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_TournamentBracketets_BracketId",
                table: "TournamentMatches",
                column: "BracketId",
                principalTable: "TournamentBracketets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentBracketets_TournamentPhases_PhaseId",
                table: "TournamentBracketets");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentBracketets_BracketId",
                table: "TournamentMatches");

            migrationBuilder.DropTable(
                name: "LessonPlayer");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RoutineExercise");

            migrationBuilder.DropTable(
                name: "RoutinePlayer");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Routines");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentBracketets",
                table: "TournamentBracketets");

            migrationBuilder.RenameTable(
                name: "TournamentBracketets",
                newName: "TournamentBracketetse");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentBracketets_PhaseId",
                table: "TournamentBracketetse",
                newName: "IX_TournamentBracketetse_PhaseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentBracketetse",
                table: "TournamentBracketetse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentBracketetse_TournamentPhases_PhaseId",
                table: "TournamentBracketetse",
                column: "PhaseId",
                principalTable: "TournamentPhases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_TournamentBracketetse_BracketId",
                table: "TournamentMatches",
                column: "BracketId",
                principalTable: "TournamentBracketetse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
