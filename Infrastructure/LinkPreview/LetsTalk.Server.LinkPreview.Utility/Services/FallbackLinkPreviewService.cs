using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using System.Text.Json;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Utility.Services
{
    public class FallbackLinkPreviewService(IDownloadService downloadService): ILinkPreviewService
    {
        private readonly IDownloadService _downloadService = downloadService;

        public async Task<LinkPreviewResponse> GenerateLinkPreviewAsync(LinkPreviewRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var encodedUrl = Uri.EscapeDataString(request.Url!);
                var apiUrl = $"https://opengraph.io/api/1.1/site/{encodedUrl}?app_id={request.SecretKey}";

                var json = await _downloadService.DownloadAsStringAsync(apiUrl, cancellationToken);
                using var doc = JsonDocument.Parse(json);

                var root = doc.RootElement;
                var data = root.GetProperty("openGraph");

                var title = data.GetProperty("title").GetString();
                var imageUrl = data.GetProperty("image").GetProperty("url").GetString();

                return new LinkPreviewResponse
                {
                    OpenGraphModel = new OpenGraphModel
                    {
                        Title = HttpUtility.HtmlDecode(title),
                        ImageUrl = imageUrl
                    }
                };
            }
            catch (Exception ex)
            {
                return new LinkPreviewResponse
                {
                    Error = ex
                };
            }
        }
    }
}
