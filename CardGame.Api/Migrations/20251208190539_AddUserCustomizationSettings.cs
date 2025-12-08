using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardGame.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCustomizationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedCardColor",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedTableColor",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedCardColor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedTableColor",
                table: "Users");
        }
    }
}
