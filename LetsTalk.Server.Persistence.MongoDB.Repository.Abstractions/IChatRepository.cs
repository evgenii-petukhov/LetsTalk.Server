using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IChatRepository
{
    Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default);

    Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default);
}
