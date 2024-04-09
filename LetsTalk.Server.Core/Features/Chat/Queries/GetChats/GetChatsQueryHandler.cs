using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Chat.Queries.GetChats;

public class GetChatsQueryHandler(
    IMapper mapper,
    IChatService contactsService,
    IAccountAgnosticService accountAgnosticService) : IRequestHandler<GetChatsQuery, List<ChatDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IChatService _contactsService = contactsService;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;

    public async Task<List<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetChatsQueryValidator(_accountAgnosticService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var accountCacheEntries = await _contactsService.GetChatsAsync(request.Id, cancellationToken);

        return _mapper.Map<List<ChatDto>>(accountCacheEntries);
    }
}
