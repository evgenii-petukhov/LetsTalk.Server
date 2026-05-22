using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImageTableAddFileStorageTypeIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileStorageTypeId",
                table: "images",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "filestoragetypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filestoragetypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "filestoragetypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Local" },
                    { 2, "AmazonS3" },
                    { 3, "AzureBlobStorage" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_images_FileStorageTypeId",
                table: "images",
                column: "FileStorageTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_images_filestoragetypes_FileStorageTypeId",
                table: "images",
                column: "FileStorageTypeId",
                principalTable: "filestoragetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_filestoragetypes_FileStorageTypeId",
                table: "images");

            migrationBuilder.DropTable(
                name: "filestoragetypes");

            migrationBuilder.DropIndex(
                name: "IX_images_FileStorageTypeId",
                table: "images");

            migrationBuilder.DropColumn(
                name: "FileStorageTypeId",
                table: "images");
        }
    }
}
