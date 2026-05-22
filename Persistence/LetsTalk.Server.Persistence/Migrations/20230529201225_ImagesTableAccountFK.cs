using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImagesTableAccountFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_ImageId",
                table: "accounts");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_ImageId",
                table: "accounts",
                column: "ImageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_ImageId",
                table: "accounts");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_ImageId",
                table: "accounts",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id");
        }
    }
}
