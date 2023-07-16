using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageTableAddImagePreviewColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImagePreviewId",
                table: "messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_ImagePreviewId",
                table: "messages",
                column: "ImagePreviewId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_images_ImagePreviewId",
                table: "messages",
                column: "ImagePreviewId",
                principalTable: "images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_images_ImagePreviewId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_ImagePreviewId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ImagePreviewId",
                table: "messages");
        }
    }
}
