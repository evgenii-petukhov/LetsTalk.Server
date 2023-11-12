using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IMessageDomainService
{
    Task SetImagePreviewAsync(Image image, int messageId);

    void MarkAsRead(int messageId);

    Task MarkAllAsRead(int recipientId, int messageId, CancellationToken cancellationToken);
}
