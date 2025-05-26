using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

[Version("0.1.0")]
[Name("LinkPreview: Create a unique index (Url)")]
public class LinkPreviewCreateUniqueIndexMigration : IMigration
{
    public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
    {
        return database.GetCollection<LinkPreview>(nameof(LinkPreview)).Indexes.CreateOneAsync(new CreateIndexModel<LinkPreview>(
            Builders<LinkPreview>.IndexKeys.Ascending(x => x.Url),
            new CreateIndexOptions
            {
                Unique = true
            }), cancellationToken: cancellationToken);
    }
}
