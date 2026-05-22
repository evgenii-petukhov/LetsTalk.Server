using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkPreviewTableUrlUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_linkpreviews_Url",
                table: "linkpreviews");

            migrationBuilder.CreateIndex(
                name: "IX_linkpreviews_Url",
                table: "linkpreviews",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_linkpreviews_Url",
                table: "linkpreviews");

            migrationBuilder.CreateIndex(
                name: "IX_linkpreviews_Url",
                table: "linkpreviews",
                column: "Url");
        }
    }
}
