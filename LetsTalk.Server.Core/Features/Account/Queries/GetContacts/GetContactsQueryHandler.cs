using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetContacts;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<AccountDto>>
{
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;

    public GetContactsQueryHandler(
        IMapper mapper,
        IAccountService accountService)
    {
        _mapper = mapper;
        _accountService = accountService;
    }

    public async Task<List<AccountDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var accountCacheEntries = await _accountService.GetContactsAsync(request.Id, cancellationToken);

        return _mapper.Map<List<AccountDto>>(accountCacheEntries);
    }
}
