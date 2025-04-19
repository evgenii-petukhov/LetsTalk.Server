using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using SimpleMongoMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

[Order(1)]
public class LinkPreviewCreateUniqueIndexMigration : IMigration
{
    public string Name => "LinkPreview: Create a unique index (Url)";

    public void Up(IMongoDatabase database)
    {
        database.GetCollection<LinkPreview>(nameof(LinkPreview)).Indexes.CreateOne(new CreateIndexModel<LinkPreview>(
            Builders<LinkPreview>.IndexKeys.Ascending(x => x.Url),
            new CreateIndexOptions
            {
                Unique = true
            }));
    }
}
