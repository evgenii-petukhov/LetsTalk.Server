using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IReadOnlyList<Message>> GetAsync(int senderId, int recipientId, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(int senderId, int recipientId, CancellationToken cancellationToken = default);

    Task SetTextHtmlAsync(int messageId, string html, CancellationToken cancellationToken = default);

    Task SetLinkPreviewAsync(int messageId, int linkPreviewId, CancellationToken cancellationToken = default);
}
