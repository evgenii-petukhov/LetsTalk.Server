using Grpc.Core;
using LetsTalk.Server.Domain;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcEndpoint;

namespace LetsTalk.Server.FileStorageService.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IImageInfoService _imageInfoService;
    private readonly IAccountRepository _accountRepository;

    public FileUploadGrpcEndpoint(
        IFileManagementService fileManagementService,
        IImageRepository imageRepository,
        IFileRepository fileRepository,
        IImageInfoService imageInfoService,
        IAccountRepository accountRepository)
    {
        _fileManagementService = fileManagementService;
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
        _imageInfoService = imageInfoService;
        _accountRepository = accountRepository;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var fileInfo = await _fileManagementService.SaveFileAsync(data, FileTypes.Image, context.CancellationToken);

        var file = await _fileRepository.CreateAsync(new Domain.File
        {
            FileName = fileInfo.FileName,
            FileTypeId = (int)FileTypes.Image,
        }, context.CancellationToken);

        var image = await _imageRepository.CreateAsync(new Image
        {
            ImageContentTypeId = (int)_imageInfoService.GetImageContentType(data),
            ImageTypeId = (int)request.ImageType,
            FileId = file.Id
        }, context.CancellationToken);

        await _accountRepository.UpdateAsync((int)context.UserState["AccountId"], null, image.Id);

        return new UploadImageResponse();
    }
}
