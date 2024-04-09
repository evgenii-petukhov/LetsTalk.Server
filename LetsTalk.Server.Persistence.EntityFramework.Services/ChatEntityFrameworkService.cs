using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class ChatEntityFrameworkService(
    IChatRepository chatRepository,
    IChatMemberRepository chatMemberRepository,
    IMapper mapper) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository = chatMemberRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var contacts = await _chatRepository.GetChatsAsync(int.Parse(accountId), cancellationToken);

        return _mapper.Map<List<ChatServiceModel>>(contacts);
    }

    public async Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var ids = await _chatMemberRepository.GetChatMemberAccountIdsAsync(int.Parse(chatId), cancellationToken);

        return ids.Select(x => x.ToString()).ToArray();
    }
}
