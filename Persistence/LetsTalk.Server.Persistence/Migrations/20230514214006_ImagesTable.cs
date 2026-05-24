using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "accounts",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "imagetypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagetypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ImageContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ImageTypeId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_images_imagecontenttypes_ImageContentTypeId",
                        column: x => x.ImageContentTypeId,
                        principalTable: "imagecontenttypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_images_imagetypes_ImageTypeId",
                        column: x => x.ImageTypeId,
                        principalTable: "imagetypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    { 3, "Gif" }
                });

            migrationBuilder.InsertData(
                table: "imagetypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Avatar" },
                    { 2, "Message" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_ImageId",
                table: "accounts",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_images_ImageContentTypeId",
                table: "images",
                column: "ImageContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_images_ImageTypeId",
                table: "images",
                column: "ImageTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "imagecontenttypes");

            migrationBuilder.DropTable(
                name: "imagetypes");

            migrationBuilder.DropIndex(
                name: "IX_accounts_ImageId",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "accounts");
        }
    }
}
