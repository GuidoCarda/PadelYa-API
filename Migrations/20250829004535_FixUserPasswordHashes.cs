using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class FixUserPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG5lfoUTu6r2+ZvrS33ePXDMdIIbp6s1lxbAH8I4hJv9JKy4nB7LEP9X8/e9ypyvsQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELbXp1J3SVCChWxMdpMp/E4oohRGiMVMVzE+gEmCNUyRmZDjDYguncfKAsuY+Qr/rA==");
        }
    }
}
