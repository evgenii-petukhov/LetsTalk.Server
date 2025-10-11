using Amazon.Lambda.Core;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using LetsTalk.Server.LinkPreview.Utility.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.LinkPreview.Lambda
{
    public static class LinkPreviewLambda
    {
        public static Task<LinkPreviewResponse> GenerateAsync(LinkPreviewRequest request)
        {
            var downloadService = new DownloadService(new FakeHttpClientService());
            var linkPreviewService = new LinkPreviewService(
                downloadService,
                new RegexService(),
                new FallbackLinkPreviewService(downloadService));
            return linkPreviewService.GenerateLinkPreviewAsync(request);
        }
    }
}
