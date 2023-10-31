using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetPagedAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default);

    Task MarkAllAsRead(int recipientId, int messageId);
}
