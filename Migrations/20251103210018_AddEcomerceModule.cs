using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEcomerceModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Paletas de pádel profesionales y amateur", true, "Paletas" },
                    { 2, "Pelotas de pádel de diferentes marcas", true, "Pelotas" },
                    { 3, "Ropa y accesorios para pádel", true, "Indumentaria" },
                    { 4, "Zapatillas especializadas para pádel", true, "Calzado" },
                    { 5, "Bolsos, grips, antivibradores y más", true, "Accesorios" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(2746), "Paleta profesional de control", "https://via.placeholder.com/300x300?text=Paleta+Head", true, "Paleta Head Delta Pro", 89990m, 15, null },
                    { 2, 1, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4602), "Paleta de potencia y control", "https://via.placeholder.com/300x300?text=Paleta+Bullpadel", true, "Paleta Bullpadel Vertex", 94990m, 10, null },
                    { 3, 1, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4605), "Paleta gama alta", "https://via.placeholder.com/300x300?text=Paleta+Nox", true, "Paleta Nox AT10", 79990m, 8, null },
                    { 4, 2, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4606), "Tubo de 3 pelotas profesionales", "https://via.placeholder.com/300x300?text=Pelotas+Head", true, "Pelotas Head Pro (x3)", 4990m, 50, null },
                    { 5, 2, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4608), "Pelotas de competición", "https://via.placeholder.com/300x300?text=Pelotas+Wilson", true, "Pelotas Wilson Tour (x3)", 4590m, 45, null },
                    { 6, 3, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4609), "Remera deportiva con tecnología dry-fit", "https://via.placeholder.com/300x300?text=Remera+Adidas", true, "Remera técnica Adidas", 12990m, 30, null },
                    { 7, 3, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4614), "Short con bolsillos especiales para pelotas", "https://via.placeholder.com/300x300?text=Short+Nike", true, "Short Nike Padel", 15990m, 25, null },
                    { 8, 4, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4616), "Zapatillas especializadas con suela clay", "https://via.placeholder.com/300x300?text=Zapatillas+Asics", true, "Zapatillas Asics Gel Padel", 45990m, 12, null },
                    { 9, 5, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4617), "Bolso paletero para 3 paletas", "https://via.placeholder.com/300x300?text=Bolso+Head", true, "Bolso Head Tour Team", 24990m, 18, null },
                    { 10, 5, new DateTime(2025, 11, 3, 21, 0, 17, 969, DateTimeKind.Utc).AddTicks(4619), "Grip premium antideslizante", "https://via.placeholder.com/300x300?text=Grip+Wilson", true, "Grip Wilson Pro", 1990m, 100, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
