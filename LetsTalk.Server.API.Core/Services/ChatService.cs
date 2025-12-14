using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.API.Core.Services;

public class ChatService(
    IChatAgnosticService chatAgnosticService,
    IMapper mapper) : IChatService
{
    private readonly IChatAgnosticService _chatAgnosticService = chatAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken)
    {
        var chats = await _chatAgnosticService.GetChatsAsync(accountId, cancellationToken);
        return _mapper.Map<List<ChatDto>>(chats);
    }

    public Task<bool> IsChatIdValidAsync(string chatId, CancellationToken cancellationToken)
    {
        return _chatAgnosticService.IsChatIdValidAsync(chatId, cancellationToken);
    }
}
