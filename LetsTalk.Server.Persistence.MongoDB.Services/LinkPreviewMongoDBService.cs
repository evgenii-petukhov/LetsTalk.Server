using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class LinkPreviewMongoDBService(ILinkPreviewRepository linkPreviewRepository) : ILinkPreviewAgnosticService
{
    private readonly ILinkPreviewRepository _linkPreviewRepository = linkPreviewRepository;

    public Task<string?> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return _linkPreviewRepository.GetIdByUrlAsync(url, cancellationToken);
    }
}
