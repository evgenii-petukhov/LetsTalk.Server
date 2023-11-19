using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class LinkPreviewEntityFrameworkService : ILinkPreviewAgnosticService
{
    private readonly ILinkPreviewRepository _linkPreviewRepository;

    public LinkPreviewEntityFrameworkService(ILinkPreviewRepository linkPreviewRepository)
    {
        _linkPreviewRepository = linkPreviewRepository;
    }

    public async Task<string?> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var linkPreviewId = await _linkPreviewRepository.GetIdByUrlAsync(url, cancellationToken);

        return linkPreviewId == 0
            ? null
            : linkPreviewId.ToString();
    }
}
