using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDBServices;

public class LinkPreviewMongoDBService : ILinkPreviewAgnosticService
{
    public Task<int> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
