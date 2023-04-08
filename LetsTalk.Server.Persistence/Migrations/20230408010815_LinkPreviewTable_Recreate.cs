using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkPreviewTable_Recreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_LinkPreview_LinkPreviewId",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LinkPreview",
                table: "LinkPreview");

            migrationBuilder.RenameTable(
                name: "LinkPreview",
                newName: "linkpreviews");

            migrationBuilder.UpdateData(
                table: "linkpreviews",
                keyColumn: "Url",
                keyValue: null,
                column: "Url",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "linkpreviews",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_linkpreviews",
                table: "linkpreviews",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_linkpreviews_Url",
                table: "linkpreviews",
                column: "Url");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_linkpreviews_LinkPreviewId",
                table: "messages",
                column: "LinkPreviewId",
                principalTable: "linkpreviews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_linkpreviews_LinkPreviewId",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_linkpreviews",
                table: "linkpreviews");

            migrationBuilder.DropIndex(
                name: "IX_linkpreviews_Url",
                table: "linkpreviews");

            migrationBuilder.RenameTable(
                name: "linkpreviews",
                newName: "LinkPreview");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "LinkPreview",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LinkPreview",
                table: "LinkPreview",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_LinkPreview_LinkPreviewId",
                table: "messages",
                column: "LinkPreviewId",
                principalTable: "LinkPreview",
                principalColumn: "Id");
        }
    }
}
