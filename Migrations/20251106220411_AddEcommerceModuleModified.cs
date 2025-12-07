using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEcommerceModuleModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paleta profesional de control", "https://via.placeholder.com/300x300?text=Paleta+Head", true, "Paleta Head Delta Pro", 89990m, 15, null },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paleta de potencia y control", "https://via.placeholder.com/300x300?text=Paleta+Bullpadel", true, "Paleta Bullpadel Vertex", 94990m, 10, null },
                    { 3, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paleta gama alta", "https://via.placeholder.com/300x300?text=Paleta+Nox", true, "Paleta Nox AT10", 79990m, 8, null },
                    { 4, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tubo de 3 pelotas profesionales", "https://via.placeholder.com/300x300?text=Pelotas+Head", true, "Pelotas Head Pro (x3)", 4990m, 50, null },
                    { 5, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pelotas de competición", "https://via.placeholder.com/300x300?text=Pelotas+Wilson", true, "Pelotas Wilson Tour (x3)", 4590m, 45, null },
                    { 6, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Remera deportiva con tecnología dry-fit", "https://via.placeholder.com/300x300?text=Remera+Adidas", true, "Remera técnica Adidas", 12990m, 30, null },
                    { 7, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Short con bolsillos especiales para pelotas", "https://via.placeholder.com/300x300?text=Short+Nike", true, "Short Nike Padel", 15990m, 25, null },
                    { 8, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zapatillas especializadas con suela clay", "https://via.placeholder.com/300x300?text=Zapatillas+Asics", true, "Zapatillas Asics Gel Padel", 45990m, 12, null },
                    { 9, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bolso paletero para 3 paletas", "https://via.placeholder.com/300x300?text=Bolso+Head", true, "Bolso Head Tour Team", 24990m, 18, null },
                    { 10, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Grip premium antideslizante", "https://via.placeholder.com/300x300?text=Grip+Wilson", true, "Grip Wilson Pro", 1990m, 100, null }
                });
        }
    }
}
