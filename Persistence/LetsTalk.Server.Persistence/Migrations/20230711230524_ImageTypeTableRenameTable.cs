using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImageTypeTableRenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_imagetypes_ImageTypeId",
                table: "images");

            migrationBuilder.DropTable(
                name: "imagetypes");

            migrationBuilder.RenameColumn(
                name: "ImageTypeId",
                table: "images",
                newName: "ImageRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_images_ImageTypeId",
                table: "images",
                newName: "IX_images_ImageRoleId");

            migrationBuilder.CreateTable(
                name: "imageroles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imageroles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "imageroles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Avatar" },
                    { 2, "Message" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_images_imageroles_ImageRoleId",
                table: "images",
                column: "ImageRoleId",
                principalTable: "imageroles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_imageroles_ImageRoleId",
                table: "images");

            migrationBuilder.DropTable(
                name: "imageroles");

            migrationBuilder.RenameColumn(
                name: "ImageRoleId",
                table: "images",
                newName: "ImageTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_images_ImageRoleId",
                table: "images",
                newName: "IX_images_ImageTypeId");

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

            migrationBuilder.InsertData(
                table: "imagetypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Avatar" },
                    { 2, "Message" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_images_imagetypes_ImageTypeId",
                table: "images",
                column: "ImageTypeId",
                principalTable: "imagetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
