using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IChatMemberRepository : IGenericRepository<Message>
{
    Task<int> GetChatMemberIdAsync(int messageId, int accountId, CancellationToken cancellationToken = default);

    Task<int[]> GetChatMemberAccountIdsAsync(int chatId, CancellationToken cancellationToken = default);
}
