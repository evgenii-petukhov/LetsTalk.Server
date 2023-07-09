using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageTableAddImageIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_ImageId",
                table: "messages",
                column: "ImageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_images_ImageId",
                table: "messages",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_images_ImageId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_ImageId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "messages");
        }
    }
}
