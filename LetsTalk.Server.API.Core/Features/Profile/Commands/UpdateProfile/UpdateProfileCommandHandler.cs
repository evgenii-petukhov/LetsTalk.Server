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
    IChatAgnosticService chatAgnosticService,
    IMapper mapper,
    IProfileCacheManager profileCacheManager,
    IAccountCacheManager accountCacheManager,
    IChatCacheManager chatCacheManager,
    IProducer<RemoveImageRequest> producer
) : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IMapper _mapper = mapper;
    private readonly IProfileCacheManager _profileCacheManager = profileCacheManager;
    private readonly IAccountCacheManager _accountCacheManager = accountCacheManager;
    private readonly IChatCacheManager _chatCacheManager = chatCacheManager;
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

        await Task.WhenAll(
            _profileCacheManager.ClearAsync(request.AccountId!),
            _accountCacheManager.ClearAsync());

        if (deletePreviousImage)
        {
            var accountIds = await _chatAgnosticService.GetAccountIdsInIndividualChatsAsync(request.AccountId!, cancellationToken);
            var removeImageRequest = _mapper.Map<RemoveImageRequest>(previousImage);
            await Task.WhenAll(
                _producer.PublishAsync(removeImageRequest, cancellationToken),
                Task.WhenAll(accountIds.Select(_chatCacheManager.ClearAsync)));
        }

        return _mapper.Map<ProfileDto>(account);
    }
}
