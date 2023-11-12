using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, AccountDto>
{
    private readonly IMapper _mapper;
    private readonly IProfileService _profileService;

    public GetProfileQueryHandler(
        IMapper mapper,
        IProfileService profileService)
    {
        _mapper = mapper;
        _profileService = profileService;
    }

    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _profileService.GetProfileAsync(request.Id, cancellationToken);
        return _mapper.Map<AccountDto>(accounts);
    }
}
