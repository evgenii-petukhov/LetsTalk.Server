using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImageContentTypeTableRenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_imagecontenttypes_ImageContentTypeId",
                table: "images");

            migrationBuilder.DropTable(
                name: "imagecontenttypes");

            migrationBuilder.RenameColumn(
                name: "ImageContentTypeId",
                table: "images",
                newName: "ImageFormatId");

            migrationBuilder.RenameIndex(
                name: "IX_images_ImageContentTypeId",
                table: "images",
                newName: "IX_images_ImageFormatId");

            migrationBuilder.CreateTable(
                name: "imageformats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imageformats", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "imageformats",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { -1, "Unknown" },
                    { 1, "Jpeg" },
                    { 2, "Png" },
                    { 3, "Gif" },
                    { 4, "Webp" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_images_imageformats_ImageFormatId",
                table: "images",
                column: "ImageFormatId",
                principalTable: "imageformats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_imageformats_ImageFormatId",
                table: "images");

            migrationBuilder.DropTable(
                name: "imageformats");

            migrationBuilder.RenameColumn(
                name: "ImageFormatId",
                table: "images",
                newName: "ImageContentTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_images_ImageFormatId",
                table: "images",
                newName: "IX_images_ImageContentTypeId");

            migrationBuilder.CreateTable(
                name: "imagecontenttypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagecontenttypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "imagecontenttypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { -1, "Unknown" },
                    { 1, "Jpeg" },
                    { 2, "Png" },
                    { 3, "Gif" },
                    { 4, "Webp" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_images_imagecontenttypes_ImageContentTypeId",
                table: "images",
                column: "ImageContentTypeId",
                principalTable: "imagecontenttypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
