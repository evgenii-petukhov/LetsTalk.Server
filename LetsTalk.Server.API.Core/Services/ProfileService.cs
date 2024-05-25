using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class ProfileService(
    IAccountAgnosticService accountAgnosticService,
    IMapper mapper) : IProfileService
{
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<ProfileDto> GetProfileAsync(string accountId, CancellationToken cancellationToken)
    {
        var accounts = await _accountAgnosticService.GetByIdAsync(accountId, cancellationToken);

        return _mapper.Map<ProfileDto>(accounts);
    }
}
