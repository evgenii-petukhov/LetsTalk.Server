using Grpc.Core;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorage.Service.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;
using AutoMapper;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using LetsTalk.Server.Caching.Abstractions;
using LetsTalk.Server.Caching.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Service.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly IImageValidationService _imageValidationService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IImageDomainService _imageDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IImageValidationService imageValidationService,
        IMapper mapper,
        IFileService fileService,
        IImageDomainService imageDomainService,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _imageService = imageService;
        _imageValidationService = imageValidationService;
        _mapper = mapper;
        _fileService = fileService;
        _imageDomainService = imageDomainService;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageRole = (ImageRoles)request.ImageRole;
        var (imageFormat, width, height) = _imageValidationService.ValidateImage(data, imageRole);

        var filename = await _fileService.SaveDataAsync(data, FileTypes.Image, imageRole, context.CancellationToken);

        var image = await _imageDomainService.CreateImageAsync(filename, imageFormat, imageRole, width, height, context.CancellationToken);

        await _unitOfWork.SaveAsync();

        return new UploadImageResponse
        {
            ImageId = image.Id
        };
    }

    public override async Task<DownloadImageResponse> DownloadImageAsync(DownloadImageRequest request, ServerCallContext context)
    {
        var model = await _cacheService.GetOrAddImageAsync(request.ImageId, async() =>
        {
            var image = await _imageService.FetchImageAsync(request.ImageId, true, context.CancellationToken);
            return _mapper.Map<ImageCacheEntry>(image);
        });

        return _mapper.Map<DownloadImageResponse>(model);
    }
}
