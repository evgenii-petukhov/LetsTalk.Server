using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;

public class GetAccountsQueryHandler(
    IMapper mapper,
    IAccountService accountService) : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IAccountService _accountService = accountService;

    public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetAccountsQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var accountCacheEntries = await _accountService.GetAccountsAsync(cancellationToken);

        return _mapper.Map<List<AccountDto>>(accountCacheEntries.Select(account => account.Id != request.Id));
    }
}
