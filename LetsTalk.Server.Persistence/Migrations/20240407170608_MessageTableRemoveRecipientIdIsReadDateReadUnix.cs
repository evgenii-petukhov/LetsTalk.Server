using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageTableRemoveRecipientIdIsReadDateReadUnix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_accounts_RecipientId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_RecipientId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "DateReadUnix",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DateReadUnix",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "messages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RecipientId",
                table: "messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_messages_RecipientId",
                table: "messages",
                column: "RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_accounts_RecipientId",
                table: "messages",
                column: "RecipientId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
