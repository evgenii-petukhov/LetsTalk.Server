using Google.Protobuf;
using Grpc.Core;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcEndpoint;

namespace LetsTalk.Server.FileStorageService.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;

    public FileUploadGrpcEndpoint(IImageService imageService)
    {
        _imageService = imageService;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var accountId = (int)context.UserState["AccountId"];
        var imageType = (ImageTypes)request.ImageType;
        var imageId = await _imageService.SaveImageAsync(request.Content.ToArray(), imageType, accountId, context.CancellationToken);

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
