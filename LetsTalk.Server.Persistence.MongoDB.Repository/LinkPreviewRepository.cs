using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class LinkPreviewRepository : ILinkPreviewRepository
{
    private readonly IMongoCollection<LinkPreview> _linkPreviewCollection;

    public LinkPreviewRepository(
        IMongoClient mongoClient,
        IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);

        _linkPreviewCollection = mongoDatabase.GetCollection<LinkPreview>(nameof(LinkPreview));
    }

    public Task<string?> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return _linkPreviewCollection
            .Find(Builders<LinkPreview>.Filter.Eq(x => x.Url, url))
            .Project(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
