using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class AccountService(
    IAccountAgnosticService accountAgnosticService,
    IMapper mapper) : IAccountService
{
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<AccountDto>> GetAccountsAsync(string id, CancellationToken cancellationToken)
    {
        var messages = await _accountAgnosticService.GetAccountsAsync(id, cancellationToken);
        return _mapper.Map<List<AccountDto>>(messages);
    }
}
