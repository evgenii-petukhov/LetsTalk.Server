using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class ContactsService(
    IAccountAgnosticService accountAgnosticService,
    IMapper mapper) : IChatService
{
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken)
    {
        var messages = await _accountAgnosticService.GetChatsAsync(accountId, cancellationToken);
        return _mapper.Map<List<ChatDto>>(messages);
    }
}
