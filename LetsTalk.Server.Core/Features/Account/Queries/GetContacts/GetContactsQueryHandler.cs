using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetContacts;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<AccountDto>>
{
    private readonly IMapper _mapper;
    private readonly IContactsService _contactsService;

    public GetContactsQueryHandler(
        IMapper mapper,
        IContactsService contactsService)
    {
        _mapper = mapper;
        _contactsService = contactsService;
    }

    public async Task<List<AccountDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var accountCacheEntries = await _contactsService.GetContactsAsync(request.Id, cancellationToken);

        return _mapper.Map<List<AccountDto>>(accountCacheEntries);
    }
}
