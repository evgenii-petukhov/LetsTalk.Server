using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class ContactsService : IContactsService
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly IMapper _mapper;

    public ContactsService(
        IAccountAgnosticService accountAgnosticService,
        IMapper mapper)
    {
        _accountAgnosticService = accountAgnosticService;
        _mapper = mapper;
    }

    public async Task<List<AccountDto>> GetContactsAsync(int accountId, CancellationToken cancellationToken)
    {
        var messages = await _accountAgnosticService.GetContactsAsync(accountId, cancellationToken);
        return _mapper.Map<List<AccountDto>>(messages);
    }
}
