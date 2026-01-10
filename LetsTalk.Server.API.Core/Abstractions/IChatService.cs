using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IChatService
{
    Task<IReadOnlyList<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken);

    Task<bool> IsChatIdValidAsync(string chatId, CancellationToken cancellationToken);

    Task<bool> IsAccountChatMemberAsync(string chatId, string accountId, CancellationToken cancellationToken = default);
}
