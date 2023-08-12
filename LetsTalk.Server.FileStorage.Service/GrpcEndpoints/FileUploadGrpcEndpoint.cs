using Grpc.Core;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Protos;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using static LetsTalk.Server.FileStorage.Service.Protos.FileUploadGrpcEndpoint;
using ImageRoles = LetsTalk.Server.Persistence.Enums.ImageRoles;
using AutoMapper;
using LetsTalk.Server.Persistence.DatabaseContext;

namespace LetsTalk.Server.FileStorage.Service.GrpcEndpoints;

public class FileUploadGrpcEndpoint : FileUploadGrpcEndpointBase
{
    private readonly IImageService _imageService;
    private readonly IImageValidationService _imageValidationService;
    private readonly IFileRepository _fileRepository;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IIOService _ioService;
    private readonly IMapper _mapper;
    private readonly LetsTalkDbContext _context;

    public FileUploadGrpcEndpoint(
        IImageService imageService,
        IImageValidationService imageValidationService,
        IFileRepository fileRepository,
        IAccountDataLayerService accountDataLayerService,
        IIOService ioService,
        IMapper mapper,
        LetsTalkDbContext context)
    {
        _imageService = imageService;
        _imageValidationService = imageValidationService;
        _fileRepository = fileRepository;
        _accountDataLayerService = accountDataLayerService;
        _ioService = ioService;
        _mapper = mapper;
        _context = context;
    }

    public override async Task<UploadImageResponse> UploadImageAsync(UploadImageRequest request, ServerCallContext context)
    {
        var data = request.Content.ToArray();
        var imageRole = (ImageRoles)request.ImageRole;
        var validationResult = _imageValidationService.ValidateImage(data, imageRole);

        var accountId = (int)context.UserState["AccountId"];

        await using var transaction = await _context.Database.BeginTransactionAsync(context.CancellationToken);

        var prevFile = imageRole == ImageRoles.Avatar ? await GetAvatarAsync(accountId, context.CancellationToken) : null;

        var imageId = await _imageService.SaveImageAsync(
            data,
            imageRole,
            validationResult.ImageFormat,
            validationResult.Width,
            validationResult.Height,
            context.CancellationToken);

        if (prevFile != null)
        {
            await _fileRepository.DeleteAsync(prevFile.Id, context.CancellationToken);
        }

        await transaction.CommitAsync(context.CancellationToken);

        if (prevFile != null)
        {
            _ioService.DeleteFile(prevFile.FileName!, FileTypes.Image);
        }

        return new UploadImageResponse
        {
            ImageId = imageId
        };
    }

    public override async Task<DownloadImageResponse> DownloadImageAsync(DownloadImageRequest request, ServerCallContext context)
    {
        var image = await _imageService.FetchImageAsync(request.ImageId, true, context.CancellationToken);
        return _mapper.Map<DownloadImageResponse>(image);
    }

    private async Task<Domain.File?> GetAvatarAsync(int accountId, CancellationToken cancellationToken)
    {
        var response = await _accountDataLayerService.GetByIdOrDefaultAsync(accountId, x => new
        {
            x.Image!.File
        }, true, cancellationToken);

        return response?.File;
    }
}
