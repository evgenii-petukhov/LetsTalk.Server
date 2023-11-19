using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class FileMongoDBService : IFileAgnosticService
{
    private readonly IUploadRepository _uploadRepository;

    public FileMongoDBService(IUploadRepository uploadRepository)
    {
        _uploadRepository = uploadRepository;
    }

    public Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _uploadRepository.DeleteByIdAsync(id, cancellationToken);
    }
}
