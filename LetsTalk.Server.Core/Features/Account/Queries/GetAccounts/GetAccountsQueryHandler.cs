using AutoMapper;
using LetsTalk.Server.Caching.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetAccountsQueryHandler(
        IAccountRepository accountRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _cacheService = cacheService;
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
