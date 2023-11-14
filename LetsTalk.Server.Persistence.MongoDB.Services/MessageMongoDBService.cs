using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class MessageMongoDBService : IMessageAgnosticService
{
    public async Task<MessageServiceModel> CreateMessageAsync(
        int senderId,
        int recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MessageServiceModel>> GetPagedAsync(
        int senderId,
        int recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(int messageId, int linkPreviewId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(
        int messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task MarkAsRead(
        int messageId,
        int recipientId,
        bool updatePreviousMessages,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
