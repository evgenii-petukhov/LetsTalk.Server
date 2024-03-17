using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChatTableImportDataFromMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO chats(SenderId, RecipientId)
SELECT SenderId, RecipientId
FROM messages
WHERE SenderId <> RecipientId
GROUP BY SenderId * RecipientId, SenderId + RecipientId;");

            migrationBuilder.Sql(@"UPDATE messages m
INNER JOIN chats c ON m.SenderId = c.SenderId AND m.RecipientId = c.RecipientId OR m.SenderId = c.RecipientId AND m.RecipientId = c.SenderId
SET m.ChatId = c.id;");

            migrationBuilder.Sql(@"INSERT INTO chatmembers(ChatId, AccountId)
SELECT	id, SenderId
FROM chats
UNION
SELECT	id, RecipientId
FROM chats;");

            migrationBuilder.Sql(@"INSERT INTO chatmessagestatuses(ChatMemberId, MessageId, IsRead, DateReadUnix)
SELECT	cm.id, m.Id, m.IsRead, m.DateReadUnix
FROM messages m
INNER JOIN chatmembers cm ON cm.ChatId = m.ChatId AND cm.AccountId = m.RecipientId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
