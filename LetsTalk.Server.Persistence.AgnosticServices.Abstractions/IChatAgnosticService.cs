using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IChatAgnosticService
{
    Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default);

    Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default);

    Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default);

    Task<string> CreateIndividualChatAsync(string[] accountIds, CancellationToken cancellationToken = default);
}
