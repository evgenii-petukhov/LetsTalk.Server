using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IMessageDomainService
{
    Task SetLinkPreviewAsync(LinkPreview linkPreview, int messageId);

    Task SetImagePreviewAsync(Image image, int messageId);

    void MarkAsRead(int messageId);

    Task MarkAllAsRead(int recipientId, int messageId);
}
