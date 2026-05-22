using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChatMessageStatusTableAddChatIdAccountIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatmessagestatuses_chatmembers_ChatMemberId",
                table: "chatmessagestatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_chatmessagestatuses",
                table: "chatmessagestatuses");

            migrationBuilder.AlterColumn<int>(
                name: "ChatMemberId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chatmessagestatuses_AccountId",
                table: "chatmessagestatuses",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_chatmessagestatuses_ChatId",
                table: "chatmessagestatuses",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_chatmessagestatuses_ChatMemberId",
                table: "chatmessagestatuses",
                column: "ChatMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_chatmessagestatuses_accounts_AccountId",
                table: "chatmessagestatuses",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chatmessagestatuses_chatmembers_ChatMemberId",
                table: "chatmessagestatuses",
                column: "ChatMemberId",
                principalTable: "chatmembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chatmessagestatuses_chats_ChatId",
                table: "chatmessagestatuses",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id");

            migrationBuilder.Sql(@"UPDATE chatmessagestatuses cms
INNER JOIN chatmembers cm ON cms.ChatMemberId = cm.Id
SET cms.ChatId = cm.ChatId, cms.AccountId = cm.AccountId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatmessagestatuses_accounts_AccountId",
                table: "chatmessagestatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_chatmessagestatuses_chatmembers_ChatMemberId",
                table: "chatmessagestatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_chatmessagestatuses_chats_ChatId",
                table: "chatmessagestatuses");

            migrationBuilder.DropIndex(
                name: "IX_chatmessagestatuses_AccountId",
                table: "chatmessagestatuses");

            migrationBuilder.DropIndex(
                name: "IX_chatmessagestatuses_ChatId",
                table: "chatmessagestatuses");

            migrationBuilder.DropIndex(
                name: "IX_chatmessagestatuses_ChatMemberId",
                table: "chatmessagestatuses");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "chatmessagestatuses");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "chatmessagestatuses");

            migrationBuilder.AlterColumn<int>(
                name: "ChatMemberId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_chatmessagestatuses",
                table: "chatmessagestatuses",
                columns: new[] { "ChatMemberId", "MessageId" });

            migrationBuilder.AddForeignKey(
                name: "FK_chatmessagestatuses_chatmembers_ChatMemberId",
                table: "chatmessagestatuses",
                column: "ChatMemberId",
                principalTable: "chatmembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
