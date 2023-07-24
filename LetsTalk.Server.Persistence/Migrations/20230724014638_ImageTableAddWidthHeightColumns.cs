using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImageTableAddWidthHeightColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "images",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "images");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "images");
        }
    }
}
