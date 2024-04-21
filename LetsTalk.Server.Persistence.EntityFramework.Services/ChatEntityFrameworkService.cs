using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class ChatEntityFrameworkService(
    IChatRepository chatRepository,
    IChatMemberRepository chatMemberRepository) : IChatAgnosticService
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository = chatMemberRepository;

    public Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        return  _chatRepository.GetChatsAsync(int.Parse(accountId), cancellationToken);
    }

    public async Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var ids = await _chatMemberRepository.GetChatMemberAccountIdsAsync(int.Parse(chatId), cancellationToken);

        return ids.Select(x => x.ToString()).ToArray();
    }
}
