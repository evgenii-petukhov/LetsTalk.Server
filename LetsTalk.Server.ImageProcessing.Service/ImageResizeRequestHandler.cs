using KafkaFlow;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestHandler(
    IImageService imageService,
    IFileService fileService,
    IImageResizeService imageResizeService,
    IOptions<FileStorageSettings> fileStorageSettings,
    IHttpClientFactory httpClientFactory,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> options) : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService = imageService;
    private readonly IFileService _fileService = fileService;
    private readonly IImageResizeService _imageResizeService = imageResizeService;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ApplicationUrlSettings _applicationUrlSettings = options.Value;

    public async Task Handle(IMessageContext context, ImageResizeRequest request)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(request.ImageId!);

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
            MessageId = request.MessageId,
            ChatId = request.ChatId,
            Filename = filename,
            Width = width,
            Height = height,
            ImageFormat = (int)ImageFormats.Webp
        };
        _signPackageService.Sign(payload);
        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");
        await _httpClientFactory.CreateClient(nameof(ImageResizeRequestHandler)).PutAsync($"{_applicationUrlSettings.Api}/api/message/setimagepreview", content);
    }
}
