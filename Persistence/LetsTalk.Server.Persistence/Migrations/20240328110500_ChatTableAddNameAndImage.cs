using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChatTableAddNameAndImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatmembers_accounts_AccountId",
                table: "chatmembers");

            migrationBuilder.DropForeignKey(
                name: "FK_chatmembers_chats_ChatId",
                table: "chatmembers");

            migrationBuilder.DropForeignKey(
                name: "FK_chats_accounts_RecipientId",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "FK_chats_accounts_SenderId",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_chats_RecipientId",
                table: "chats");

            migrationBuilder.DropIndex(
                name: "IX_chats_SenderId",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "chats");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "messages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "chats",
                type: "varchar(36)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsIndividual",
                table: "chats",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "chats",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "chatmembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "chatmembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chats_ImageId",
                table: "chats",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_chatmembers_accounts_AccountId",
                table: "chatmembers",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chatmembers_chats_ChatId",
                table: "chatmembers",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chats_images_ImageId",
                table: "chats",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"UPDATE chats
SET IsIndividual = TRUE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatmembers_accounts_AccountId",
                table: "chatmembers");

            migrationBuilder.DropForeignKey(
                name: "FK_chatmembers_chats_ChatId",
                table: "chatmembers");

            migrationBuilder.DropForeignKey(
                name: "FK_chats_images_ImageId",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_chats_ImageId",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "IsIndividual",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "chats");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "messages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RecipientId",
                table: "chats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "chats",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "chatmembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "chatmembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_chats_RecipientId",
                table: "chats",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_chats_SenderId",
                table: "chats",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_chatmembers_accounts_AccountId",
                table: "chatmembers",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chatmembers_chats_ChatId",
                table: "chatmembers",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chats_accounts_RecipientId",
                table: "chats",
                column: "RecipientId",
                principalTable: "accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chats_accounts_SenderId",
                table: "chats",
                column: "SenderId",
                principalTable: "accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id");
        }
    }
}
