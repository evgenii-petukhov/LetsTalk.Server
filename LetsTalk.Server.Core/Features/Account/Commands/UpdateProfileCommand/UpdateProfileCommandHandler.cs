using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly IMapper _mapper;
    private readonly IProfileCacheManager _profileCacheManager;

    public UpdateProfileCommandHandler(
        IAccountAgnosticService accountAgnosticService,
        IMapper mapper,
        IProfileCacheManager profileCacheManager)
    {
        _accountAgnosticService = accountAgnosticService;
        _mapper = mapper;
        _profileCacheManager = profileCacheManager;
    }

    public async Task<AccountDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var account = await _accountAgnosticService.UpdateProfileAsync(
            request.AccountId!.Value,
            request.FirstName!,
            request.LastName!,
            request.Email!,
            request.ImageId,
            cancellationToken);

        await _profileCacheManager.RemoveAsync(request.AccountId!.Value);

        return _mapper.Map<AccountDto>(account);
    }
}
