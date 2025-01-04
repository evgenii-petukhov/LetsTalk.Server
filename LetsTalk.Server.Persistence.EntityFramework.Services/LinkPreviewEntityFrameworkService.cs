using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class LinkPreviewEntityFrameworkService(ILinkPreviewRepository linkPreviewRepository) : ILinkPreviewAgnosticService
{
    private readonly ILinkPreviewRepository _linkPreviewRepository = linkPreviewRepository;

    public async Task<string?> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var linkPreviewId = await _linkPreviewRepository.GetIdByUrlAsync(url, cancellationToken);

        return linkPreviewId == 0
            ? null
            : linkPreviewId.ToString(CultureInfo.InvariantCulture);
    }
}
