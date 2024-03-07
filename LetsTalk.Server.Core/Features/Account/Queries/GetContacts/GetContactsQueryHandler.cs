using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetContacts;

public class GetContactsQueryHandler(
    IMapper mapper,
    IContactsService contactsService,
    IAccountAgnosticService accountAgnosticService) : IRequestHandler<GetContactsQuery, List<AccountDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IContactsService _contactsService = contactsService;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;

    public async Task<List<AccountDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetContactsQueryValidator(_accountAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var accountCacheEntries = await _contactsService.GetContactsAsync(request.Id, cancellationToken);

        return _mapper.Map<List<AccountDto>>(accountCacheEntries);
    }
}
