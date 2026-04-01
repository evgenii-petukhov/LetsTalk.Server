using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.API.Core.Services;

public class ProfileService(
    IProfileAgnosticService profileAgnosticService,
    IMapper mapper) : IProfileService
{
    private readonly IProfileAgnosticService _profileAgnosticService = profileAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<ProfileDto> GetProfileAsync(string accountId, CancellationToken cancellationToken)
    {
        var accounts = await _profileAgnosticService.GetByIdAsync(accountId, cancellationToken);

        return _mapper.Map<ProfileDto>(accounts);
    }
}
