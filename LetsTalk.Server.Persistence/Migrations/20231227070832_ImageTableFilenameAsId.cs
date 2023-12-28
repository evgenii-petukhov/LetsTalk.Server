using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImageTableFilenameAsId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_images_ImageId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_images_files_FileId",
                table: "images");

            migrationBuilder.DropForeignKey(
                name: "FK_images_imageroles_ImageRoleId",
                table: "images");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePreviewId",
                table: "messages",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "messages",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "images",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "accounts",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_images_ImageId",
                table: "messages",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id",
                onUpdate: ReferentialAction.Cascade,
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_images_ImageId",
                table: "accounts",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id",
                onUpdate: ReferentialAction.Cascade,
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_images_ImagePreviewId",
                table: "messages",
                column: "ImagePreviewId",
                principalTable: "images",
                principalColumn: "Id",
                onUpdate: ReferentialAction.Cascade,
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql(@"update images i
inner join files f on f.Id = i.FileId
SET i.Id = FileName;");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "imageroles");

            migrationBuilder.DropTable(
                name: "filetypes");

            migrationBuilder.DropIndex(
                name: "IX_images_FileId",
                table: "images");

            migrationBuilder.DropIndex(
                name: "IX_images_ImageRoleId",
                table: "images");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "images");

            migrationBuilder.DropColumn(
                name: "ImageRoleId",
                table: "images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ImagePreviewId",
                table: "messages",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "messages",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "images",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageRoleId",
                table: "images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "filetypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filetypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileTypeId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_files_filetypes_FileTypeId",
                        column: x => x.FileTypeId,
                        principalTable: "filetypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "filetypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Image" },
                    { 2, "Audio" }
                });

            migrationBuilder.InsertData(
                table: "imageroles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Avatar" },
                    { 2, "Message" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_images_FileId",
                table: "images",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_images_ImageRoleId",
                table: "images",
                column: "ImageRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_files_FileTypeId",
                table: "files",
                column: "FileTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_images_files_FileId",
                table: "images",
                column: "FileId",
                principalTable: "files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_images_imageroles_ImageRoleId",
                table: "images",
                column: "ImageRoleId",
                principalTable: "imageroles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
