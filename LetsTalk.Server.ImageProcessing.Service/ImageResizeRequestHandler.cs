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

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>, IDisposable
{
    private readonly IImageService _imageService;
    private readonly IFileService _fileService;
    private readonly IImageResizeService _imageResizeService;

    private readonly ISignPackageService _signPackageService;
    private readonly KafkaSettings _kafkaSettings;
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly HttpClient _httpClient;
    private readonly ApplicationUrlSettings _applicationUrlSettings;
    private bool _disposedValue;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IFileService fileService,
        IImageResizeService imageResizeService,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<FileStorageSettings> fileStorageSettings,
        IHttpClientFactory httpClientFactory,
        ISignPackageService signPackageService,
        IOptions<ApplicationUrlSettings> options)
    {
        _imageService = imageService;
        _fileService = fileService;
        _imageResizeService = imageResizeService;
        _kafkaSettings = kafkaSettings.Value;
        _fileStorageSettings = fileStorageSettings.Value;
        _signPackageService = signPackageService;
        _httpClient = httpClientFactory.CreateClient(nameof(ImageResizeRequestHandler));
        _applicationUrlSettings = options.Value;
    }

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
        using var content = GetHttpContent(setImagePreviewRequest);
        await _httpClient.PutAsync($"{_applicationUrlSettings.Api}/api/message/setimagepreview", content);
    }

    private HttpContent GetHttpContent(object payload)
    {
        return new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");
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
