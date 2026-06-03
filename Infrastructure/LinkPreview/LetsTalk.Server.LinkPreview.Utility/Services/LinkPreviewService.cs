using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Utility.Services;

public class LinkPreviewService(
    IDownloadService downloadService,
    IRegexService regexService,
    ILinkPreviewService linkPreviewService,
    ILinkPreviewLogger linkPreviewLogger) : ILinkPreviewService
{
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IRegexService _regexService = regexService;
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;
    private readonly ILinkPreviewLogger _linkPreviewLogger = linkPreviewLogger;

    public async Task<OpenGraphModel> GenerateLinkPreviewAsync(
        LinkPreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        _linkPreviewLogger.LogInformation($"Generating link preview for URL: {request.Url}");
        try
        {
            _linkPreviewLogger.LogInformation("Downloading page content...");
            var pageString = await _downloadService.DownloadAsStringAsync(request.Url!, cancellationToken);

            _linkPreviewLogger.LogInformation("Parsing Open Graph data...");
            var model = _regexService.GetOpenGraphModel(pageString);

            return new OpenGraphModel
            {
                Title = HttpUtility.HtmlDecode(model.Title),
                ImageUrl = model.ImageUrl
            };
        }
        catch (HttpRequestException e)
        when (e.StatusCode == System.Net.HttpStatusCode.Forbidden && !string.IsNullOrWhiteSpace(request.SecretKey))
        {
            _linkPreviewLogger.LogError(e, "Forbidden error occurred. Trying fallback service.");
            return await _linkPreviewService.GenerateLinkPreviewAsync(request, cancellationToken);
        }
    }
}
