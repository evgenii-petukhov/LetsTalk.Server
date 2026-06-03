using Amazon.Lambda.Core;
using LetsTalk.Server.API.Client;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using LetsTalk.Server.LinkPreview.Utility.Services;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.LinkPreview.Lambda
{
    public static class LinkPreviewLambda
    {
        public static async Task GenerateAsync(LinkPreviewRequest request, ILambdaContext context)
        {
            var serializedRequest = JsonSerializer.Serialize(request);
            context.Logger.LogLine($"Request received: {serializedRequest}");

            var httpClientService = new FakeHttpClientService();
            var downloadService = new DownloadService(httpClientService);
            var logger = new AmazonLambdaLogger(context.Logger);

            var linkPreviewService = new LinkPreviewService(
                downloadService,
                new RegexService(),
                new FallbackLinkPreviewService(downloadService, logger),
                logger);

            var model = await linkPreviewService.GenerateLinkPreviewAsync(request);

            if (model == null || string.IsNullOrWhiteSpace(model.Title))
            {
                context.Logger.LogInformation($"Title is empty: {request.Url}");
                return;
            }

            var payload = new SetLinkPreviewRequest
            {
                MessageId = request.MessageId,
                ChatId = request.ChatId,
                Url = request.Url,
                Title = model.Title,
                ImageUrl = model.ImageUrl
            };
            using var client = httpClientService.GetHttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Token}");
            var apiClient = new ApiClient(request.ApiUrl, client);
            await apiClient.SetLinkPreviewAsync(payload);

            context.Logger.LogInformation($"New LinkPreview added: {request.Url}");
        }
    }
}
