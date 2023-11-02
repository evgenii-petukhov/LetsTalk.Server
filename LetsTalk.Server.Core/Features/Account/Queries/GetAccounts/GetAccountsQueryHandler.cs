using AutoMapper;
using LetsTalk.Server.Configuration.Models;
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
    private readonly IMemoryCache _memoryCache;

    public GetAccountsQueryHandler(
        IAccountRepository accountRepository,
        IMapper mapper,
        IMemoryCache memoryCache,
        IOptions<MessagingSettings> messagingSettings)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(messagingSettings.Value.ContactsCacheLifeTimeInSeconds);
    }

    public Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(request.Id, async cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            var accounts = await _accountRepository.GetContactsAsync(request.Id, cancellationToken);
            return _mapper.Map<List<AccountDto>>(accounts);
        })!;

    }
}
