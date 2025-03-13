using Grpc.Core;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Protos;
using static LetsTalk.Server.FileStorage.Service.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;
using AutoMapper;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.GrpcEndpoints;

public class FileUploadGrpcEndpoint(
    IImageValidationService imageValidationService,
    IMapper mapper,
    IAgnosticFileService fileService,
    IImageService imageService) : FileUploadGrpcEndpointBase
{
    private readonly IImageValidationService _imageValidationService = imageValidationService;
    private readonly IMapper _mapper = mapper;
    private readonly IAgnosticFileService _fileService = fileService;
    private readonly IImageService _imageService = imageService;

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageRole = (ImageRoles)request.ImageRole;
        var (imageFormat, width, height) = _imageValidationService.ValidateImage(data, imageRole);

        var filename = await _fileService.SaveDataAsync(data, FileTypes.Image, width, height, context.CancellationToken);
        await _fileService.SaveImageInfoAsync(filename, width, height, context.CancellationToken);

        return new UploadImageResponse
        {
            Id = filename,
            Width = width,
            Height = height,
            ImageFormat = (int)imageFormat
        };
    }

    public override async Task<DownloadImageResponse> DownloadImageAsync(DownloadImageRequest request, ServerCallContext context)
    {
        var model = await _imageService.FetchImageAsync(request.ImageId, context.CancellationToken);

        return _mapper.Map<DownloadImageResponse>(model);
    }
}
