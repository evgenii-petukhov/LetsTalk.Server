using Amazon.Lambda.Core;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using LetsTalk.Server.LinkPreview.Utility.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.LinkPreview.Lambda
{
    public static class LinkPreviewLambda
    {
        public static Task<LinkPreviewResponse> GenerateAsync(string input)
        {
            var linkPreviewService = new LinkPreviewService(
                new DownloadService(new FakeHttpClientService()),
                new RegexService());
            return linkPreviewService.GenerateLinkPreviewAsync(input);
        }
    }
}
