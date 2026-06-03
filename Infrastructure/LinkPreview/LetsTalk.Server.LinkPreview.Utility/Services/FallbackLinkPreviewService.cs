using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using System.Text.Json;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Utility.Services
{
    public class FallbackLinkPreviewService(
        IDownloadService downloadService,
        ILinkPreviewLogger linkPreviewLogger) : ILinkPreviewService
    {
        private readonly IDownloadService _downloadService = downloadService;
        private readonly ILinkPreviewLogger _linkPreviewLogger = linkPreviewLogger;

        public async Task<OpenGraphModel> GenerateLinkPreviewAsync(
            LinkPreviewRequest request,
            CancellationToken cancellationToken)
        {
            _linkPreviewLogger.LogInformation("Generating link preview using fallback service");

            var encodedUrl = Uri.EscapeDataString(request.Url!);
            var apiUrl = $"https://opengraph.io/api/1.1/site/{encodedUrl}?app_id={request.SecretKey}";

            _linkPreviewLogger.LogInformation("Downloading Open Graph data from fallback service...");
            var json = await _downloadService.DownloadAsStringAsync(apiUrl, cancellationToken);

            _linkPreviewLogger.LogInformation("Parsing Open Graph data...");
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var data = root.GetProperty("openGraph");
            var title = data.GetProperty("title").GetString();
            var imageUrl = data.GetProperty("image").GetProperty("url").GetString();

            return new OpenGraphModel
            {
                Title = HttpUtility.HtmlDecode(title),
                ImageUrl = imageUrl
            };
        }
    }
}
