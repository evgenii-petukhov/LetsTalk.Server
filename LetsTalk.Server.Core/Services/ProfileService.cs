using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class ProfileService : IProfileService
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly IMapper _mapper;

    public ProfileService(
        IAccountAgnosticService accountAgnosticService,
        IMapper mapper)
    {
        _accountAgnosticService = accountAgnosticService;
        _mapper = mapper;
    }

    public async Task<AccountDto> GetProfileAsync(int accountId, CancellationToken cancellationToken)
    {
        var accounts = await _accountAgnosticService.GetByIdAsync(accountId, cancellationToken);

        return _mapper.Map<AccountDto>(accounts);
    }
}
