using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IMessageAgnosticService
{
    Task<MessageAgnosticModel> CreateMessageAsync(
        int senderId,
        int recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken);

    Task<List<MessageAgnosticModel>> GetPagedAsync(
        int senderId,
        int recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default);

    Task SetLinkPreviewAsync(int messageId, int linkPreviewId, CancellationToken cancellationToken = default);

    Task SetLinkPreviewAsync(
        int messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default);
}
