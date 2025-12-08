using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGame.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddXpAndLevelToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperiencePoints",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperiencePoints",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Users");
        }
    }
}
