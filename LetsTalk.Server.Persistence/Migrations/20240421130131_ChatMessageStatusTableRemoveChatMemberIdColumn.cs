using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChatMessageStatusTableRemoveChatMemberIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "IX_chatmessagestatuses_ChatId",
                table: "chatmessagestatuses");

            migrationBuilder.DropIndex(
                name: "IX_chatmessagestatuses_ChatMemberId",
                table: "chatmessagestatuses");

            migrationBuilder.DropColumn(
                name: "ChatMemberId",
                table: "chatmessagestatuses");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
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
                columns: new[] { "ChatId", "AccountId", "MessageId" });

            migrationBuilder.AddForeignKey(
                name: "FK_chatmessagestatuses_accounts_AccountId",
                table: "chatmessagestatuses",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chatmessagestatuses_chats_ChatId",
                table: "chatmessagestatuses",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatmessagestatuses_accounts_AccountId",
                table: "chatmessagestatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_chatmessagestatuses_chats_ChatId",
                table: "chatmessagestatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_chatmessagestatuses",
                table: "chatmessagestatuses");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ChatMemberId",
                table: "chatmessagestatuses",
                type: "int",
                nullable: true);

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
        }
    }
}
