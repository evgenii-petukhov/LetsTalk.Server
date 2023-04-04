using AutoMapper;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Notifications.Abstractions;

namespace LetsTalk.Server.Notifications.Handlers;

public class LinkPreviewNotificationHandler : IMessageHandler<LinkPreviewNotification>
{
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public LinkPreviewNotificationHandler(
        INotificationService notificationService,
        IMapper mapper)
    {
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task Handle(IMessageContext context, LinkPreviewNotification notification)
    {
        var notificationDto = _mapper.Map<LinkPreviewNotificationDto>(notification);
        await _notificationService.SendLinkPreviewNotification(notification.RecipientId, notificationDto);
    }
}
