using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class TournamentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if EnrollmentDate column already exists before adding it
            migrationBuilder.Sql(@"
                IF COL_LENGTH('TournamentEnrollments','EnrollmentDate') IS NULL
                BEGIN
                    ALTER TABLE [TournamentEnrollments]
                    ADD [EnrollmentDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000'
                END
            ");

            // Check if PlayerId column already exists before adding it
            migrationBuilder.Sql(@"
                IF COL_LENGTH('TournamentEnrollments','PlayerId') IS NULL
                BEGIN
                    ALTER TABLE [TournamentEnrollments]
                    ADD [PlayerId] int NOT NULL DEFAULT 0
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "TournamentEnrollments");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "TournamentEnrollments");
        }
    }
}
