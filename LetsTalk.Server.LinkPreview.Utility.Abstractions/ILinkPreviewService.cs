using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Utility.Abstractions;

public interface ILinkPreviewService
{
    Task<LinkPreviewResponse> GenerateLinkPreviewAsync(string url, CancellationToken cancellationToken = default);
}
