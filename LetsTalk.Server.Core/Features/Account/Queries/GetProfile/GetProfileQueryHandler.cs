using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, AccountDto>
{
    private readonly IMapper _mapper;
    private readonly IProfileService _profileService;
    private readonly IAccountAgnosticService _accountAgnosticService;

    public GetProfileQueryHandler(
        IMapper mapper,
        IProfileService profileService,
        IAccountAgnosticService accountAgnosticService)
    {
        _mapper = mapper;
        _profileService = profileService;
        _accountAgnosticService = accountAgnosticService;
    }

    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetProfileQueryValidator(_accountAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var accounts = await _profileService.GetProfileAsync(request.Id, cancellationToken);
        return _mapper.Map<AccountDto>(accounts);
    }
}
