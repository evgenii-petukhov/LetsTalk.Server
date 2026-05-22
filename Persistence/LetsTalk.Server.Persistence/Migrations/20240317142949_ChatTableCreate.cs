using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChatTableCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    RecipientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chats_accounts_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chats_accounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "accounts",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chatmembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChatId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatmembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chatmembers_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chatmembers_chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "chats",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chatmessagestatuses",
                columns: table => new
                {
                    ChatMemberId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DateReadUnix = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatmessagestatuses", x => new { x.ChatMemberId, x.MessageId });
                    table.ForeignKey(
                        name: "FK_chatmessagestatuses_chatmembers_ChatMemberId",
                        column: x => x.ChatMemberId,
                        principalTable: "chatmembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chatmessagestatuses_messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_messages_ChatId",
                table: "messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_chatmembers_AccountId",
                table: "chatmembers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_chatmembers_ChatId",
                table: "chatmembers",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_chatmessagestatuses_MessageId",
                table: "chatmessagestatuses",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_chats_RecipientId",
                table: "chats",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_chats_SenderId",
                table: "chats",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages");

            migrationBuilder.DropTable(
                name: "chatmessagestatuses");

            migrationBuilder.DropTable(
                name: "chatmembers");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropIndex(
                name: "IX_messages_ChatId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "messages");
        }
    }
}
