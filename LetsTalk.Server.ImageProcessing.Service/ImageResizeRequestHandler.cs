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
    IOptions<ApplicationUrlSettings> options) : IMessageHandler<ImageResizeRequest>, IDisposable
{
    private readonly IImageService _imageService = imageService;
    private readonly IFileService _fileService = fileService;
    private readonly IImageResizeService _imageResizeService = imageResizeService;

    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(ImageResizeRequestHandler));
    private readonly ApplicationUrlSettings _applicationUrlSettings = options.Value;
    private bool _disposedValue;

    public async Task Handle(IMessageContext context, ImageResizeRequest imageResizeRequest)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(imageResizeRequest.ImageId!);

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


        var setImagePreviewRequest = new SetImagePreviewRequest
        {
            MessageId = imageResizeRequest.MessageId,
            ChatId = imageResizeRequest.ChatId,
            Filename = filename,
            Width = width,
            Height = height,
            ImageFormat = ImageFormats.Webp
        };
        _signPackageService.Sign(setImagePreviewRequest);
        using var content = new StringContent(
            JsonSerializer.Serialize(setImagePreviewRequest),
            Encoding.UTF8,
            "application/json");
        await _httpClient.PutAsync($"{_applicationUrlSettings.Api}/api/message/setimagepreview", content);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
