using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IChatRepository : IGenericRepository<Chat>
{
    Task<List<ChatServiceModel>> GetChatsAsync(int accountId, CancellationToken cancellationToken = default);

    Task<bool> IsChatIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task<Chat> GetIndividualChatByAccountIdsAsync(int[] accountIds, CancellationToken cancellationToken = default);

    Task<ChatServiceModel> GetChatServiceModelAsync(int chatId, int accountId, CancellationToken cancellationToken = default);
}
