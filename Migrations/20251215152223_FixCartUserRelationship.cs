using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class FixCartUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BookingId",
                table: "Challenges",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Bookings_BookingId",
                table: "Challenges",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Bookings_BookingId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BookingId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Challenges");
        }
    }
}
