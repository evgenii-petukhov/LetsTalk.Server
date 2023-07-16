using Google.Protobuf;
using Grpc.Core;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorage.Service.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;

namespace LetsTalk.Server.FileStorage.Service.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly IImageValidationService _imageValidationService;
    private readonly IFileRepository _fileRepository;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IIOService _ioService;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IImageValidationService imageValidationService,
        IFileRepository fileRepository,
        IAccountDataLayerService accountDataLayerService,
        IIOService ioService)
    {
        _imageService = imageService;
        _imageValidationService = imageValidationService;
        _fileRepository = fileRepository;
        _accountDataLayerService = accountDataLayerService;
        _ioService = ioService;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageRole = (ImageRoles)request.ImageRole;
        var validationResult =  _imageValidationService.ValidateImage(data, imageRole);

        var accountId = (int)context.UserState["AccountId"];

        var previousAvatarFile = imageRole == ImageRoles.Avatar ? await GetAvatarAsync(accountId) : null;

        var imageId = await _imageService.SaveImageAsync(data, imageRole, validationResult.ImageFormat, context.CancellationToken);

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
