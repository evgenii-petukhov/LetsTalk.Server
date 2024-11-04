using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator(ILinkPreviewService linkPreviewService) : ILinkPreviewService
{
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;

    public Task<LinkPreviewResponse> GenerateLinkPreviewAsync(string url, CancellationToken cancellationToken)
    {
        return _linkPreviewService.GenerateLinkPreviewAsync(url, cancellationToken);
    }
}
