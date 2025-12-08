using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonRoutineAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonRoutineAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    RoutineId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByTeacherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonRoutineAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonRoutineAssignments_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonRoutineAssignments_Persons_AssignedByTeacherId",
                        column: x => x.AssignedByTeacherId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonRoutineAssignments_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LessonRoutineAssignments_Routines_RoutineId",
                        column: x => x.RoutineId,
                        principalTable: "Routines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonRoutineAssignments_AssignedByTeacherId",
                table: "LessonRoutineAssignments",
                column: "AssignedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonRoutineAssignments_LessonId_PersonId",
                table: "LessonRoutineAssignments",
                columns: new[] { "LessonId", "PersonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonRoutineAssignments_PersonId",
                table: "LessonRoutineAssignments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonRoutineAssignments_RoutineId",
                table: "LessonRoutineAssignments",
                column: "RoutineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonRoutineAssignments");
        }
    }
}
