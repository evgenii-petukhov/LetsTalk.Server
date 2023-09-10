using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IReadOnlyList<Message>> GetPagedAsTrackingAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(int senderId, int recipientId, CancellationToken cancellationToken = default);

    Task SetLinkPreviewAsync(int messageId, int linkPreviewId, CancellationToken cancellationToken = default);
}
