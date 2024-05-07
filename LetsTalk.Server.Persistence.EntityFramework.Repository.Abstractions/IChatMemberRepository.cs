using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IChatMemberRepository : IGenericRepository<Message>
{
    Task<int[]> GetChatMemberAccountIdsAsync(int chatId, CancellationToken cancellationToken = default);
}
