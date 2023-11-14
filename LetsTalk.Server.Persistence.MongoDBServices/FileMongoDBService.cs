using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDBServices;

public class FileMongoDBService : IFileAgnosticService
{
    public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
