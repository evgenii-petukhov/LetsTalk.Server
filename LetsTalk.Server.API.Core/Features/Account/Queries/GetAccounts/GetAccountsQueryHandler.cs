using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Account.Queries.GetAccounts;

public class GetAccountsQueryHandler(
    IAccountService accountService) : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    private readonly IAccountService _accountService = accountService;

    public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accountCacheEntries = await _accountService.GetAccountsAsync(cancellationToken);

        return accountCacheEntries
            .Where(account => account.Id != request.Id)
            .ToList();
    }
}
