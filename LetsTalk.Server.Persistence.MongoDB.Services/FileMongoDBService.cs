using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class FileMongoDBService : IFileAgnosticService
{
    public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
