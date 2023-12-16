using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using MongoDBMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

public class LinkPreviewCreateUniqueIndexMigration : IMigration
{
    public MongoDBMigrations.Version Version => new(0, 1, 0);

    public string Name => "LinkPreview: Create a unique index (Url)";

    public void Down(IMongoDatabase database)
    {
        throw new NotImplementedException();
    }

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
