using Amazon.Lambda.Core;
using System.Text.Json;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using LetsTalk.Server.LinkPreview.Utility.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.LinkPreview.Lambda
{
    public static class LinkPreviewLambda
    {
        public static Task<LinkPreviewResponse> GenerateAsync(object request, ILambdaContext context)
        {
            var serializedRequest = JsonSerializer.Serialize(request);
            context.Logger.LogLine($"Request received: {serializedRequest}");

            var linkPreviewRequest = request as LinkPreviewRequest;

            if (linkPreviewRequest == null)
            {
                return null!;
            }

            var downloadService = new DownloadService(new FakeHttpClientService());
            var linkPreviewService = new LinkPreviewService(
                downloadService,
                new RegexService(),
                new FallbackLinkPreviewService(downloadService));
            return linkPreviewService.GenerateLinkPreviewAsync(linkPreviewRequest);
        }
    }
}
