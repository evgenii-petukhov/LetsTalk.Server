using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class ProfileService : IProfileService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public ProfileService(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountDto> GetProfileAsync(int accountId, CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        return _mapper.Map<AccountDto>(accounts);
    }
}
