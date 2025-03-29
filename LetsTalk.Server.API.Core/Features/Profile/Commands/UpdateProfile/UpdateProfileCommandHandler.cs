using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(
    IAccountAgnosticService accountAgnosticService,
    IMapper mapper,
    IProfileCacheManager profileCacheManager,
    IProducer<RemoveImageRequest> producer
) : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IMapper _mapper = mapper;
    private readonly IProfileCacheManager _profileCacheManager = profileCacheManager;
    private readonly IProducer<RemoveImageRequest> _producer = producer;

    public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountAgnosticService.GetByIdAsync(request.AccountId!, cancellationToken);

        var previousImage = account.Image;
        var deletePreviousImage = previousImage != null && request.Image != null;

        account = request.Image == null
            ? await _accountAgnosticService.UpdateProfileAsync(
                request.AccountId!,
                request.FirstName!,
                request.LastName!,
                cancellationToken)
            : await _accountAgnosticService.UpdateProfileAsync(
                request.AccountId!,
                request.FirstName!,
                request.LastName!,
                request.Image.Id!,
                request.Image.Width,
                request.Image.Height,
                (ImageFormats)request.Image.ImageFormat,
                (FileStorageTypes)request.Image.FileStorageTypeId,
                cancellationToken);

        await _profileCacheManager.ClearAsync(request.AccountId!);

        if (deletePreviousImage)
        {
            var removeImageRequest = _mapper.Map<RemoveImageRequest>(previousImage);
            await _producer.PublishAsync(removeImageRequest, cancellationToken);
        }

        return _mapper.Map<ProfileDto>(account);
    }
}
