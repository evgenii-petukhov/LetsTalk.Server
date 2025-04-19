using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using SimpleMongoMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

[Order(3)]
public class ChatMessageStatusCreateUniqueIndexMigration : IMigration
{
    public string Name => "ChatMessageStatus: Create a unique index (ChatId + AccountId + MessageId)";

    public void Down(IMongoDatabase database)
    {
        throw new NotImplementedException();
    }

    public void Up(IMongoDatabase database)
    {
        database.GetCollection<ChatMessageStatus>(nameof(ChatMessageStatus)).Indexes.CreateOne(new CreateIndexModel<ChatMessageStatus>(
            Builders<ChatMessageStatus>.IndexKeys
                .Ascending(x => x.ChatId)
                .Ascending(x => x.AccountId)
                .Ascending(x => x.MessageId),
            new CreateIndexOptions
            {
                Unique = true,
            }));
    }
}