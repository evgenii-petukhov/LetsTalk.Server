using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

[Version("0.1.2")]
[Name("ChatMessageStatus: Create a unique index (ChatId + AccountId + MessageId)")]
public class ChatMessageStatusCreateUniqueIndexMigration : IMigration
{
    public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
    {
        return database.GetCollection<ChatMessageStatus>(nameof(ChatMessageStatus)).Indexes.CreateOneAsync(new CreateIndexModel<ChatMessageStatus>(
            Builders<ChatMessageStatus>.IndexKeys
                .Ascending(x => x.ChatId)
                .Ascending(x => x.AccountId)
                .Ascending(x => x.MessageId),
            new CreateIndexOptions
            {
                Unique = true,
            }), cancellationToken: cancellationToken);
    }
}