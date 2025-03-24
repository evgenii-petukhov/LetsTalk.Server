using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.Infrastructure.ApiClient;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.SignPackage.Abstractions;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestConsumer(
    IImageProcessingService imageProcessingService,
    IHttpClientFactory httpClientFactory,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> applicationUrlOptions,
    IOptions<FileStorageSettings> fileStorageOptions) : IConsumer<ImageResizeRequest>
{
    private readonly IImageProcessingService _imageProcessingService = imageProcessingService;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ApplicationUrlSettings _applicationUrlSettings = applicationUrlOptions.Value;
    private readonly FileStorageSettings _fileStorageSettings = fileStorageOptions.Value;

    public async Task Consume(ConsumeContext<ImageResizeRequest> context)
    {
        var response = await _imageProcessingService.ProcessImageAsync(
            context.Message.ImageId!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight,
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
        _signPackageService.Sign(payload);
        using var client = _httpClientFactory.CreateClient(nameof(ImageResizeRequestConsumer));
        var apiClient = new ApiClient(_applicationUrlSettings.Api, client);
        await apiClient.SetImagePreviewAsync(payload);
    }
}
