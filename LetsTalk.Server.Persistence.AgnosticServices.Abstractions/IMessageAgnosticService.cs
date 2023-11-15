using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IMessageAgnosticService
{
    Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken);

    Task<List<MessageServiceModel>> GetPagedAsync(
        string senderId,
        string recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default);

    Task<MessageServiceModel> SetLinkPreviewAsync(string messageId, int linkPreviewId, CancellationToken cancellationToken = default);

    Task<MessageServiceModel> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default);

    Task MarkAsRead(
        int messageId,
        string recipientId,
        bool updatePreviousMessages,
        CancellationToken cancellationToken);
}
