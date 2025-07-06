using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCourtBookingEntityName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CourtBookings_CourtBookingId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "CourtBookings");

            migrationBuilder.RenameColumn(
                name: "CourtBookingId",
                table: "Payments",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_CourtBookingId",
                table: "Payments",
                newName: "IX_Payments_BookingId");

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtSlotId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_CourtSlots_CourtSlotId",
                        column: x => x.CourtSlotId,
                        principalTable: "CourtSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourtSlotId",
                table: "Bookings",
                column: "CourtSlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PersonId",
                table: "Bookings",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Payments",
                newName: "CourtBookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                newName: "IX_Payments_CourtBookingId");

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
                name: "IX_CourtBookings_CourtSlotId",
                table: "CourtBookings",
                column: "CourtSlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourtBookings_PersonId",
                table: "CourtBookings",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CourtBookings_CourtBookingId",
                table: "Payments",
                column: "CourtBookingId",
                principalTable: "CourtBookings",
                principalColumn: "Id");
        }
    }
}
