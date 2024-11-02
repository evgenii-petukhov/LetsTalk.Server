using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using MongoDBMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

public class ChatMessageStatusCreateUniqueIndexMigration : IMigration
{
    public MongoDBMigrations.Version Version => new(0, 1, 2);

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
