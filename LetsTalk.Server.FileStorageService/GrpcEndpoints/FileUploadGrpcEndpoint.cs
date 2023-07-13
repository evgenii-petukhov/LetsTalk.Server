using Google.Protobuf;
using Grpc.Core;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using LetsTalk.Server.ImageProcessor.Models;
using Microsoft.Extensions.Options;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;

namespace LetsTalk.Server.FileStorageService.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly IImageInfoService _imageInfoService;
    private readonly KafkaSettings _kafkaSettings;
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly IMessageProducer _messageProducer;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IImageInfoService imageInfoService,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _imageService = imageService;
        _imageInfoService = imageInfoService;
        _kafkaSettings = kafkaSettings.Value;
        _fileStorageSettings = fileStorageSettings.Value;
        _messageProducer = producerAccessor.GetProducer(_kafkaSettings.ImageResizeRequest!.Producer);
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageFormat = _imageInfoService.GetImageFormat(data);
        if (imageFormat != Persistence.Enums.ImageFormats.Webp)
        {
            throw new BadRequestException("Image format is not supported");
        }

        var accountId = (int)context.UserState["AccountId"];
        var imageRole = (ImageRoles)request.ImageRole;
        var imageId = await _imageService.SaveImageAsync(data, imageRole, imageFormat, accountId, context.CancellationToken);

        if (request.ImageRole == Protos.ImageRoles.Message)
        {
            await _messageProducer.ProduceAsync(
                _kafkaSettings.ImageResizeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ImageResizeRequest
                {
                    ImageId = imageId,
                    MaxWidth = _fileStorageSettings.PictureMaxWidth,
                    MaxHeight = _fileStorageSettings.PictureMaxHeight
                });
        }

        return new UploadImageResponse
        {
            ImageId = imageId
        };
    }

    public override async Task<DownloadImageResponse> DownloadImageAsync(DownloadImageRequest request, ServerCallContext context)
    {
        var content = await _imageService.FetchImageAsync(request.ImageId, context.CancellationToken);

        return new DownloadImageResponse
        {
            Content = ByteString.CopyFrom(content)
        };
    }
}
