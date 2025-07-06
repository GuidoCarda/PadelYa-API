using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class NewDomainDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TournamentBracketets_TournamentPhases_PhaseId",
                table: "TournamentBracketets");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentBracketets_BracketId",
                table: "TournamentMatches");

            migrationBuilder.DropTable(
                name: "LessonPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentBracketets",
                table: "TournamentBracketets");

            migrationBuilder.RenameTable(
                name: "TournamentBracketets",
                newName: "TournamentBrackets");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentBracketets_PhaseId",
                table: "TournamentBrackets",
                newName: "IX_TournamentBrackets_PhaseId");

            migrationBuilder.AddColumn<int>(
                name: "CourtSlotId",
                table: "TournamentMatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CourtBookingId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LessonEnrollmentId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TournamentEnrollmentId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourtSlotId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentBrackets",
                table: "TournamentBrackets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CourtSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtSlots_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonEnrollments_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonEnrollments_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourtBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtSlotId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourtBookings_CourtSlots_CourtSlotId",
                        column: x => x.CourtSlotId,
                        principalTable: "CourtSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourtBookings_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMatches_CourtSlotId",
                table: "TournamentMatches",
                column: "CourtSlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CourtBookingId",
                table: "Payments",
                column: "CourtBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LessonEnrollmentId",
                table: "Payments",
                column: "LessonEnrollmentId",
                unique: true,
                filter: "[LessonEnrollmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PersonId",
                table: "Payments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TournamentEnrollmentId",
                table: "Payments",
                column: "TournamentEnrollmentId",
                unique: true,
                filter: "[TournamentEnrollmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourtSlotId",
                table: "Lessons",
                column: "CourtSlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_PlayerId",
                table: "Lessons",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtBookings_CourtSlotId",
                table: "CourtBookings",
                column: "CourtSlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourtBookings_PersonId",
                table: "CourtBookings",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtSlots_CourtId",
                table: "CourtSlots",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonEnrollments_LessonId",
                table: "LessonEnrollments",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonEnrollments_PersonId",
                table: "LessonEnrollments",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_CourtSlots_CourtSlotId",
                table: "Lessons",
                column: "CourtSlotId",
                principalTable: "CourtSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Person_PlayerId",
                table: "Lessons",
                column: "PlayerId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CourtBookings_CourtBookingId",
                table: "Payments",
                column: "CourtBookingId",
                principalTable: "CourtBookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_LessonEnrollments_LessonEnrollmentId",
                table: "Payments",
                column: "LessonEnrollmentId",
                principalTable: "LessonEnrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Person_PersonId",
                table: "Payments",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TournamentEnrollments_TournamentEnrollmentId",
                table: "Payments",
                column: "TournamentEnrollmentId",
                principalTable: "TournamentEnrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentBrackets_TournamentPhases_PhaseId",
                table: "TournamentBrackets",
                column: "PhaseId",
                principalTable: "TournamentPhases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentMatches_CourtSlots_CourtSlotId",
                table: "TournamentMatches",
                column: "CourtSlotId",
                principalTable: "CourtSlots",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_CourtSlots_CourtSlotId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Person_PlayerId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CourtBookings_CourtBookingId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_LessonEnrollments_LessonEnrollmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Person_PersonId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TournamentEnrollments_TournamentEnrollmentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentBrackets_TournamentPhases_PhaseId",
                table: "TournamentBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_CourtSlots_CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentMatches_TournamentBrackets_BracketId",
                table: "TournamentMatches");

            migrationBuilder.DropTable(
                name: "CourtBookings");

            migrationBuilder.DropTable(
                name: "LessonEnrollments");

            migrationBuilder.DropTable(
                name: "CourtSlots");

            migrationBuilder.DropIndex(
                name: "IX_TournamentMatches_CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CourtBookingId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_LessonEnrollmentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PersonId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TournamentEnrollmentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_CourtSlotId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_PlayerId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentBrackets",
                table: "TournamentBrackets");

            migrationBuilder.DropColumn(
                name: "CourtSlotId",
                table: "TournamentMatches");

            migrationBuilder.DropColumn(
                name: "CourtBookingId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LessonEnrollmentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TournamentEnrollmentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CourtSlotId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "TournamentBrackets",
                newName: "TournamentBracketets");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentBrackets_PhaseId",
                table: "TournamentBracketets",
                newName: "IX_TournamentBracketets_PhaseId");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionId",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentBracketets",
                table: "TournamentBracketets",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlayer_PlayerId",
                table: "LessonPlayer",
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
    }
}
