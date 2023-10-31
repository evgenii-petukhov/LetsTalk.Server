using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

public class MessageDomainService: IMessageDomainService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IEntityFactory _entityFactory;

    public MessageDomainService(
        IMessageRepository messageRepository,
        IEntityFactory entityFactory)
    {
        _messageRepository = messageRepository;
        _entityFactory = entityFactory;
    }

    public async Task SetLinkPreviewAsync(LinkPreview linkPreview, int messageId)
    {
        var message = await _messageRepository.GetByIdAsTrackingAsync(messageId);
        message.SetLinkPreview(linkPreview);
    }

    public async Task SetImagePreviewAsync(Image image, int messageId)
    {
        var message = await _messageRepository.GetByIdAsTrackingAsync(messageId);
        message.SetImagePreview(image);
    }

    public void MarkAsRead(int messageId)
    {
        var message = _entityFactory.CreateMessage(messageId);
        message.MarkAsRead();
    }

    public Task MarkAllAsRead(int recipientId, int messageId)
    {
        return _messageRepository.MarkAllAsRead(recipientId, messageId);
    }
}
