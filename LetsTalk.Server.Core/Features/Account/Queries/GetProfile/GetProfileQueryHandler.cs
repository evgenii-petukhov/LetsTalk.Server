using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, AccountDto>
{
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;

    public GetProfileQueryHandler(
        IMapper mapper,
        IAccountService accountService)
    {
        _mapper = mapper;
        _accountService = accountService;
    }

    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetProfileAsync(request.Id, cancellationToken);
        return _mapper.Map<AccountDto>(accounts);
    }
}
