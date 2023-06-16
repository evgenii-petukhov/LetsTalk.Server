using Grpc.Core;
using LetsTalk.Server.FileStorageService.Protos;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcService;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileUploadGrpcService : FileUploadGrpcServiceBase
{
    public override Task<FileUploadResponse> UploadAsync(FileUploadRequest request, ServerCallContext context)
    {
        return Task.FromResult(new FileUploadResponse());
    }
}
