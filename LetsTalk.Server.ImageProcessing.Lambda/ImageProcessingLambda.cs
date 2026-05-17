using Amazon.Lambda.Core;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine;
using LetsTalk.Server.ImageProcessing.Utility;
using LetsTalk.Server.ImageProcessing.Utility.Models;
using LetsTalk.Server.Infrastructure.ApiClient;
using LetsTalk.Server.Persistence.Enums;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.ImageProcessing.Lambda
{
    public static class ImageProcessingLambda
    {
        public static async Task ProcessImageAsync(ProcessImageRequest request, ILambdaContext context)
        {
            var serializedRequest = JsonSerializer.Serialize(request);
            context.Logger.LogLine($"Request received: {serializedRequest}");

            var imageProcessingService = new ImageProcessingService(
                new FakeFileServiceResolver(request.BucketName!),
                new ImageResizeService());

            var response = await imageProcessingService.ProcessImageAsync(
                request.FileName!,
                request.MaxWidth,
                request.MaxHeight,
                (FileStorageTypes)request.FileStorageTypeId);

            var payload = new SetImagePreviewRequest
            {
                MessageId = request.MessageId,
                ChatId = request.ChatId,
                Filename = response.Filename,
                Width = response.Width,
                Height = response.Height,
                ImageFormat = (int)ImageFormats.Webp,
                FileStorageTypeId = request.FileStorageTypeId
            };

            var httpClientService = new FakeHttpClientService();
            using var client = httpClientService.GetHttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Token}");
            var apiClient = new ApiClient(request.ApiUrl, client);
            await apiClient.SetImagePreviewAsync(payload);
        }
    }
}
