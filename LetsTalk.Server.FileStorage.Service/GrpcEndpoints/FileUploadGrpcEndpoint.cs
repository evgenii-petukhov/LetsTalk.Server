using Google.Protobuf;
using Grpc.Core;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using static LetsTalk.Server.FileStorage.Service.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;

namespace LetsTalk.Server.FileStorage.Service.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IImageValidationService _imageValidationService;
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly IMessageProducer _messageProducer;
    private readonly IFileRepository _fileRepository;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IIOService _ioService;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IProducerAccessor producerAccessor,
        IImageValidationService imageValidationService,
        IFileRepository fileRepository,
        IAccountDataLayerService accountDataLayerService,
        IIOService ioService,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _imageService = imageService;
        _imageValidationService = imageValidationService;
        _kafkaSettings = kafkaSettings.Value;
        _fileStorageSettings = fileStorageSettings.Value;
        _fileRepository = fileRepository;
        _accountDataLayerService = accountDataLayerService;
        _ioService = ioService;
        _messageProducer = producerAccessor.GetProducer(_kafkaSettings.ImageResizeRequest!.Producer);
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageRole = (ImageRoles)request.ImageRole;
        var validationResult =  _imageValidationService.ValidateImage(data, imageRole);

        var accountId = (int)context.UserState["AccountId"];

        var previousAvatarFile = imageRole == ImageRoles.Avatar ? await GetAvatarAsync(accountId) : null;

        var imageId = await _imageService.SaveImageAsync(data, imageRole, validationResult.ImageFormat, context.CancellationToken);

        if (imageRole == ImageRoles.Message)
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

        if (imageRole == ImageRoles.Avatar && previousAvatarFile != null)
        {
            _ioService.DeleteFile(previousAvatarFile.FileName!, FileTypes.Image);
            await _fileRepository.DeleteAsync(previousAvatarFile.Id);
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

    private async Task<Domain.File?> GetAvatarAsync(int accountId)
    {
        var response = await _accountDataLayerService.GetByIdOrDefaultAsync(accountId, x => new
        {
            x.Image!.File
        }, true);

        return response?.File;
    }
}
