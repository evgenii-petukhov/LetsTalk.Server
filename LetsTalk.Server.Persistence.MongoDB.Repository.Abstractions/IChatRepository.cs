using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;
namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IChatRepository
{
    Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default);

    Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default);

    Task<Chat> GetIndividualChatByAccountIdsAsync(string[] accountIds, CancellationToken cancellationToken = default);

    Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default);

    Task<Chat> CreateIndividualChatAsync(string[] accountIds, CancellationToken cancellationToken = default);
}
