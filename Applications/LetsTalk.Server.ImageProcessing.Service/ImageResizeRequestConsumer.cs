using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.API.Client;
using LetsTalk.Server.Models.Kafka;
using LetsTalk.Server.Persistence.Enums;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestConsumer(
    IImageProcessingService imageProcessingService,
    IHttpClientFactory httpClientFactory,
    IOptions<ApplicationUrlSettings> applicationUrlOptions) : IConsumer<ImageResizeRequest>
{
    private readonly IImageProcessingService _imageProcessingService = imageProcessingService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ApplicationUrlSettings _applicationUrlSettings = applicationUrlOptions.Value;

    public async Task Consume(ConsumeContext<ImageResizeRequest> context)
    {
        var response = await _imageProcessingService.ProcessImageAsync(
            context.Message.ImageId!,
            context.Message.MaxWidth,
            context.Message.MaxHeight,
            (FileStorageTypes)context.Message.FileStorageTypeId,
            context.CancellationToken);

        var payload = new SetImagePreviewRequest
        {
            MessageId = context.Message.MessageId,
            ChatId = context.Message.ChatId,
            Filename = response.Filename,
            Width = response.Width,
            Height = response.Height,
            ImageFormat = (int)ImageFormats.Webp,
            FileStorageTypeId = context.Message.FileStorageTypeId
        };
        using var client = _httpClientFactory.CreateClient(nameof(ImageResizeRequestConsumer));
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {context.Message.Token}");
        var apiClient = new ApiClient(_applicationUrlSettings.Api, client);
        await apiClient.SetImagePreviewAsync(payload, context.CancellationToken);
    }
}
