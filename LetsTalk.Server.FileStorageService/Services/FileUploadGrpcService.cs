using Grpc.Core;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcService;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileUploadGrpcService : FileUploadGrpcServiceBase
{
    private readonly IFileManagementService _fileManagementService;

    public FileUploadGrpcService(
        IFileManagementService fileManagementService)
    {
        _fileManagementService = fileManagementService;
    }

    public override Task<FileUploadResponse> UploadAsync(FileUploadRequest request, ServerCallContext context)
    {
        _fileManagementService.SaveFileAsync(request.Content.ToArray(), FileStorage.Models.FileStorageItemType.Image);
        return Task.FromResult(new FileUploadResponse());
    }
}
