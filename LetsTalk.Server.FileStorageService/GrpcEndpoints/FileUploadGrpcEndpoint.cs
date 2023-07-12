using Google.Protobuf;
using Grpc.Core;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Protos;
using static LetsTalk.Server.FileStorageService.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;

namespace LetsTalk.Server.FileStorageService.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly IImageInfoService _imageInfoService;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IImageInfoService imageInfoService)
    {
        _imageService = imageService;
        _imageInfoService = imageInfoService;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageFormat = _imageInfoService.GetImageFormat(data);
        if (imageFormat != Persistence.Enums.ImageFormats.Webp)
        {
            throw new BadRequestException("Image format is not supported");
        }

        var accountId = (int)context.UserState["AccountId"];
        var imageRole = (ImageRoles)request.ImageRole;
        var imageId = await _imageService.SaveImageAsync(data, imageRole, imageFormat, accountId, context.CancellationToken);

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
