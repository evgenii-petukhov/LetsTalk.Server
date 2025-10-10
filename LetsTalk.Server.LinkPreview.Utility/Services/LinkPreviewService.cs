using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Utility.Services;

public class LinkPreviewService(
    IDownloadService downloadService,
    IRegexService regexService,
    ILinkPreviewService linkPreviewService) : ILinkPreviewService
{
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IRegexService _regexService = regexService;
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;

    public async Task<LinkPreviewResponse> GenerateLinkPreviewAsync(LinkPreviewRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var pageString = await _downloadService.DownloadAsStringAsync(request.Url!, cancellationToken);
            var model = _regexService.GetOpenGraphModel(pageString);

            return new LinkPreviewResponse
            {
                OpenGraphModel = model == null
                    ? null!
                    : new OpenGraphModel
                    {
                        Title = HttpUtility.HtmlDecode(model.Title),
                        ImageUrl = model.ImageUrl
                    }
            };
        }
        catch (HttpRequestException e)
        when (e.StatusCode == System.Net.HttpStatusCode.Forbidden && !string.IsNullOrWhiteSpace(request.SecretKey))
        {
            return await _linkPreviewService.GenerateLinkPreviewAsync(request, cancellationToken);
        }
    }
}
