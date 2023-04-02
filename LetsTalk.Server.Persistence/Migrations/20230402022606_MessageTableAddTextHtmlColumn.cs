using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageTableAddTextHtmlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextHtml",
                table: "messages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextHtml",
                table: "messages");
        }
    }
}
