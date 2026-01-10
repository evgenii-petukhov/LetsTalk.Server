using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IChatAgnosticService
{
    Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default);

    Task<List<string>> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default);

    Task<List<string>> GetAccountIdsInIndividualChatsAsync(string accountId, CancellationToken cancellationToken = default);

    Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default);

    Task<string> CreateIndividualChatAsync(IEnumerable<string> accountIds, CancellationToken cancellationToken = default);

    Task<bool> IsAccountChatMemberAsync(string chatId, string accountId, CancellationToken cancellationToken = default);
}
