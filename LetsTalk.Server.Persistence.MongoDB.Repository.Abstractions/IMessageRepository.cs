using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IMessageRepository
{
    Task<List<Message>> GetPagedAsync(
        string senderId,
        string recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default);

    Task<Message> CreateAsync(
        string senderId,
        string recipientId,
        string text,
        string textHtml,
        string imageId,
        CancellationToken cancellationToken = default);

    Task<Message> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(string messageId, CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(string senderId, string recipientId, long dateCreatedUnix, CancellationToken cancellationToken = default);

    Task<Message> SetLinkPreviewAsync(string messageId, string linkPreviewId, CancellationToken cancellationToken = default);

    Task<Message> SetImagePreviewAsync(string messageId, string imageId, CancellationToken cancellationToken = default);

    Task<Message> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default);
}
