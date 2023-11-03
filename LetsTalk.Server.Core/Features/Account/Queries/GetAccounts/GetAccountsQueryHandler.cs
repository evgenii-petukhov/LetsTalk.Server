using AutoMapper;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    private readonly TimeSpan _cacheLifeTimeInSeconds;

    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetAccountsQueryHandler(
        IAccountRepository accountRepository,
        IMapper mapper,
        ICacheService cacheService,
        IOptions<CachingSettings> cachingSettings)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _cacheService = cacheService;
        _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ContactsCacheLifeTimeInSeconds);
    }

    public Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        return _cacheService.GetOrAddAccountsAsync(request.Id, async () =>
        {
            var accounts = await _accountRepository.GetContactsAsync(request.Id, cancellationToken);
            return _mapper.Map<List<AccountDto>>(accounts);
        })!;
    }
}
