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
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken = default);
}
