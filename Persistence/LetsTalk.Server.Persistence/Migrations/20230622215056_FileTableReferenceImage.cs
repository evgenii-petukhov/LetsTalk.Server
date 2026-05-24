using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FileTableReferenceImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "images");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "files",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_images_FileId",
                table: "images",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_images_files_FileId",
                table: "images",
                column: "FileId",
                principalTable: "files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_files_FileId",
                table: "images");

            migrationBuilder.DropIndex(
                name: "IX_images_FileId",
                table: "images");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "images");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "files");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "images",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
