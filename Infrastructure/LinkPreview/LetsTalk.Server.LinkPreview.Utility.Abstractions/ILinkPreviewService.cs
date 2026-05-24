using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Utility.Abstractions;

public interface ILinkPreviewService
{
    Task<LinkPreviewResponse> GenerateLinkPreviewAsync(LinkPreviewRequest request, CancellationToken cancellationToken = default);
}
