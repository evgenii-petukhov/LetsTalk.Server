﻿using Google.Protobuf;
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
        var image = await _imageRepository.GetByIdWithFileAsync(request.ImageId, context.CancellationToken);
        var content = await _fileManagementService.GetFileContentAsync(image!.File!.FileName!, FileTypes.Image, context.CancellationToken);

        return new DownloadImageResponse
        {
            Content = ByteString.CopyFrom(content)
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