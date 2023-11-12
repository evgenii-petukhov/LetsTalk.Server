using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class ContactsService : IContactsService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public ContactsService(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<List<AccountDto>> GetContactsAsync(int accountId, CancellationToken cancellationToken)
    {
        var messages = await _accountRepository.GetContactsAsync(accountId, cancellationToken);
        return _mapper.Map<List<AccountDto>>(messages);
    }
}
