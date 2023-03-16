using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IReadOnlyList<Message>> GetAsync(int senderId, int recipientId);

    Task MarkAsReadAsync(int messageId, int recipientId);
}
