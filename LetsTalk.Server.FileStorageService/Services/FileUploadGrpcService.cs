using Grpc.Core;
using LetsTalk.Server.Domain;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcService;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileUploadGrpcService : FileUploadGrpcServiceBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IImageInfoService _imageInfoService;

    public FileUploadGrpcService(
        IFileManagementService fileManagementService,
        IImageRepository imageRepository,
        IFileRepository fileRepository,
        IImageInfoService imageInfoService)
    {
        _fileManagementService = fileManagementService;
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
        _imageInfoService = imageInfoService;
    }

    public override async Task<FileUploadResponse> UploadAsync(FileUploadRequest request, ServerCallContext context)
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

        return new FileUploadResponse
        {
            FileId = image.Id
        };
    }
}
