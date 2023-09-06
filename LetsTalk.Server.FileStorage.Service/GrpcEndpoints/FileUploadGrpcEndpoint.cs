using Grpc.Core;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorage.Service.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;
using AutoMapper;

namespace LetsTalk.Server.FileStorage.Service.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly IImageValidationService _imageValidationService;
    private readonly IAccountRepository _accountRepository;
    private readonly IIOService _ioService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IImageDataLayerService _imageDataLayerService;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IImageValidationService imageValidationService,
        IAccountRepository accountRepository,
        IIOService ioService,
        IMapper mapper,
        IFileService fileService,
        IImageDataLayerService imageDataLayerService)
    {
        _imageService = imageService;
        _imageValidationService = imageValidationService;
        _accountRepository = accountRepository;
        _ioService = ioService;
        _mapper = mapper;
        _fileService = fileService;
        _imageDataLayerService = imageDataLayerService;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageRole = (ImageRoles)request.ImageRole;
        var validationResult = _imageValidationService.ValidateImage(data, imageRole);

        var accountId = (int)context.UserState["AccountId"];
        var prevFile = imageRole == ImageRoles.Avatar ? await GetAvatarAsync(accountId, context.CancellationToken) : null;
        var filename = await _fileService.SaveDataAsync(data, FileTypes.Image, imageRole, context.CancellationToken);

        var image = prevFile == null
            ? await _imageDataLayerService.CreateImageAsync(filename, validationResult.ImageFormat, imageRole, validationResult.Width, validationResult.Height, context.CancellationToken)
            : await _imageDataLayerService.ReplaceImageAsync(filename, validationResult.ImageFormat, imageRole, validationResult.Width, validationResult.Height, prevFile.Id, context.CancellationToken);

        if (prevFile != null)
        {
            _ioService.DeleteFile(prevFile.FileName!, FileTypes.Image);
        }

        return new UploadImageResponse
        {
            ImageId = image.Id
        };
    }

    public override async Task<DownloadImageResponse> DownloadImageAsync(DownloadImageRequest request, ServerCallContext context)
    {
        var image = await _imageService.FetchImageAsync(request.ImageId, true, context.CancellationToken);
        return _mapper.Map<DownloadImageResponse>(image);
    }

    private async Task<Domain.File?> GetAvatarAsync(int accountId, CancellationToken cancellationToken)
    {
        var response = await _accountRepository.GetByIdOrDefaultAsync(accountId, x => new
        {
            x.Image!.File
        }, true, cancellationToken);

        return response?.File;
    }
}
