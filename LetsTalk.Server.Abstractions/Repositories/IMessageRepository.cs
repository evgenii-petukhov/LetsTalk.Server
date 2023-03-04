using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Abstractions.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IReadOnlyList<Message>> GetAsync(int senderId, int recipientId);

    Task MarkAsReadAsync(int messageId, int recipientId);
}
