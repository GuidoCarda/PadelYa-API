using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToTournamentEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferenceId",
                table: "TournamentEnrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TournamentEnrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferenceId",
                table: "TournamentEnrollments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TournamentEnrollments");
        }
    }
}
