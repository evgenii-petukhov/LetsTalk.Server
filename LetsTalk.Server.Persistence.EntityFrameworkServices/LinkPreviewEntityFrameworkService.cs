using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class LinkPreviewEntityFrameworkService : ILinkPreviewAgnosticService
{
    private readonly ILinkPreviewRepository _linkPreviewRepository;

    public LinkPreviewEntityFrameworkService(ILinkPreviewRepository linkPreviewRepository)
    {
        _linkPreviewRepository = linkPreviewRepository;
    }

    public Task<int> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return _linkPreviewRepository.GetIdByUrlAsync(url, cancellationToken);
    }
}
