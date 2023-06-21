using Grpc.Core;
using LetsTalk.Server.Domain;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Models;
using LetsTalk.Server.FileStorageService.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcService;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileUploadGrpcService : FileUploadGrpcServiceBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly IImageRepository _imageRepository;
    private readonly IImageInfoService _imageInfoService;

    public FileUploadGrpcService(
        IFileManagementService fileManagementService,
        IImageRepository imageRepository,
        IImageInfoService imageInfoService)
    {
        _fileManagementService = fileManagementService;
        _imageRepository = imageRepository;
        _imageInfoService = imageInfoService;
    }

    public override async Task<FileUploadResponse> UploadAsync(FileUploadRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var fileInfo = await _fileManagementService.SaveFileAsync(data, FileStorageItemType.Image, context.CancellationToken);

        var file = await _imageRepository.CreateAsync(new Image
        {
            FileName = fileInfo.FileName,
            ImageContentTypeId = (int)_imageInfoService.GetImageContentType(data),
            ImageTypeId = (int)request.ImageType
        }, context.CancellationToken);

        return new FileUploadResponse
        {
            FileId = file.Id
        };
    }
}
