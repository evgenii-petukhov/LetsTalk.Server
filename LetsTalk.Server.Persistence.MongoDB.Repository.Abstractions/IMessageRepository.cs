using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IMessageRepository
{
    Task<List<Message>> GetPagedAsync(
        string chatId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default);

    Task<Message> CreateAsync(
        string senderId,
        string chatId,
        string text,
        string textHtml,
        string linkPreviewId,
        CancellationToken cancellationToken = default);

    Task<Message> CreateAsync(
        string senderId,
        string chatId,
        string text,
        string textHtml,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default);

    Task<Message> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(string chatId, string accountId, string messageId, CancellationToken cancellationToken = default);

    Task<Message> SetLinkPreviewAsync(string messageId, string linkPreviewId, CancellationToken cancellationToken = default);

    Task<Message> SetImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default);

    Task<Message> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default);
}
