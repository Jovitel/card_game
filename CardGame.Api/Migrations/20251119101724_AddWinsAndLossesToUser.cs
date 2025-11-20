using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGame.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWinsAndLossesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Losses",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Losses",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Users");
        }
    }
}
