using Google.Protobuf;
using Grpc.Core;
using LetsTalk.Server.Domain;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using System.Diagnostics;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcEndpoint;

namespace LetsTalk.Server.FileStorageService.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IImageInfoService _imageInfoService;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<FileUploadGrpcEndpoint> _logger;

    public FileUploadGrpcEndpoint(
        IFileManagementService fileManagementService,
        IImageRepository imageRepository,
        IFileRepository fileRepository,
        IImageInfoService imageInfoService,
        IAccountRepository accountRepository,
        ILogger<FileUploadGrpcEndpoint> logger)
    {
        _fileManagementService = fileManagementService;
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
        _imageInfoService = imageInfoService;
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var accountId = (int)context.UserState["AccountId"];
        if (request.ImageType == UploadImageRequest.Types.ImageType.Avatar)
        {
            await RemoveAvatarAsync(accountId, context.CancellationToken);
        }

        var data = request.Content.ToArray();
        var filename = await _fileManagementService.SaveDataAsync(data, FileTypes.Image, context.CancellationToken);

        var file = await _fileRepository.CreateAsync(new Domain.File
        {
            FileName = filename,
            FileTypeId = (int)FileTypes.Image,
        }, context.CancellationToken);

        var image = await _imageRepository.CreateAsync(new Image
        {
            ImageContentTypeId = (int)_imageInfoService.GetImageContentType(data),
            ImageTypeId = (int)request.ImageType,
            FileId = file.Id
        }, context.CancellationToken);

        await _accountRepository.SetImageIdAsync(accountId, image.Id, context.CancellationToken);

        return new UploadImageResponse();
    }

    public override async Task<DownloadImageResponse> DownloadImageAsync(DownloadImageRequest request, ServerCallContext context)
    {
        var sw = new Stopwatch();
        sw.Start();
        var image = await _imageRepository.GetByIdWithFileAsync(request.ImageId, context.CancellationToken);
        sw.Stop();
        _logger.LogInformation("File Id resolved: {imageId} => {fileId}: {time} ms", request.ImageId, image!.FileId, sw.ElapsedMilliseconds);
        sw.Restart();
        var content = await _fileManagementService.GetFileContentAsync(image!.File!.FileName!, FileTypes.Image, context.CancellationToken);
        sw.Stop();
        _logger.LogInformation("File read: {imageId} => {fileId}: {time} ms", request.ImageId, image!.FileId, sw.ElapsedMilliseconds);
        sw.Restart();
        var bytes = ByteString.CopyFrom(content);
        sw.Stop();
        _logger.LogInformation("Content converted: {imageId} => {fileId}: {time} ms", request.ImageId, image!.FileId, sw.ElapsedMilliseconds);
        return new DownloadImageResponse
        {
            Content = bytes
        };
    }

    private async Task RemoveAvatarAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdIncludingFilesAsync(accountId, cancellationToken);
        var file = account!.Image?.File;
        if (file != null)
        {
            _fileManagementService.DeleteFile(file!.FileName!, FileTypes.Image);
            await _fileRepository.DeleteAsync(file.Id, cancellationToken);
        }
    }
}
