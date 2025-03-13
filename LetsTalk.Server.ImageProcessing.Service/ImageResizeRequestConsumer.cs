using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.Infrastructure.ApiClient;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.SignPackage.Abstractions;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestConsumer(
    IImageService imageService,
    IFileService fileService,
    IImageResizeService imageResizeService,
    IOptions<FileStorageSettings> fileStorageSettings,
    IHttpClientFactory httpClientFactory,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> options) : IConsumer<ImageResizeRequest>
{
    private readonly IImageService _imageService = imageService;
    private readonly IFileService _fileService = fileService;
    private readonly IImageResizeService _imageResizeService = imageResizeService;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ApplicationUrlSettings _applicationUrlSettings = options.Value;

    public async Task Consume(ConsumeContext<ImageResizeRequest> context)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(context.Message.ImageId!);

        if (fetchImageResponse == null)
        {
            return;
        }

        var (data, width, height) = _imageResizeService.Resize(
            fetchImageResponse.Content!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);

        var filename = await _fileService.SaveDataAsync(data!, FileTypes.Image, width, height);
        await _fileService.SaveImageInfoAsync(filename, width, height);

        var payload = new SetImagePreviewRequest
        {
            MessageId = context.Message.MessageId,
            ChatId = context.Message.ChatId,
            Filename = filename,
            Width = width,
            Height = height,
            ImageFormat = (int)ImageFormats.Webp
        };
        _signPackageService.Sign(payload);
        using var client = _httpClientFactory.CreateClient(nameof(ImageResizeRequestConsumer));
        var apiClient = new ApiClient(_applicationUrlSettings.Api, client);
        await apiClient.SetImagePreviewAsync(payload);
    }
}
