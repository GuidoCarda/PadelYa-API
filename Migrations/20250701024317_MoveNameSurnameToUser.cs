using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace padelya_api.Migrations
{
    /// <inheritdoc />
    public partial class MoveNameSurnameToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First add the new columns to Users table
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Copy data from Person to User
            migrationBuilder.Sql(@"
                UPDATE Users 
                SET Name = p.Name, Surname = p.Surname 
                FROM Users u 
                INNER JOIN Person p ON u.PersonId = p.Id 
                WHERE u.PersonId IS NOT NULL
            ");

            // Now drop the columns from Person table
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Person");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add columns back to Person table
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Person",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Person",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Copy data back from User to Person
            migrationBuilder.Sql(@"
                UPDATE Person 
                SET Name = u.Name, Surname = u.Surname 
                FROM Person p 
                INNER JOIN Users u ON p.Id = u.PersonId 
                WHERE u.PersonId IS NOT NULL
            ");

            // Drop columns from Users table
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Users");
        }
    }
}
