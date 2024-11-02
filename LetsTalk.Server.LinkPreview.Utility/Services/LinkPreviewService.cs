using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Utility.Services;

public class LinkPreviewService(
    IDownloadService downloadService,
    IRegexService regexService) : ILinkPreviewService
{
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IRegexService _regexService = regexService;

    public async Task<LinkPreviewResponse> GenerateLinkPreviewAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var pageString = await _downloadService.DownloadAsStringAsync(url, cancellationToken);

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
        catch(Exception e)
        {
            return new LinkPreviewResponse
            {
                Exception = e
            };
        }
    }
}
