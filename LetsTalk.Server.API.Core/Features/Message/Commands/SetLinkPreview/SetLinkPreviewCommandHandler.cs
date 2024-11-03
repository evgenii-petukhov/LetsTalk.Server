using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;

public class SetLinkPreviewCommandHandler : IRequestHandler<SetLinkPreviewCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService;
    private readonly IChatAgnosticService _chatAgnosticService;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _linkPreviewNotificationProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageCacheManager _messageCacheManager;

    public SetLinkPreviewCommandHandler(
        IMessageAgnosticService messageAgnosticService,
        IChatAgnosticService chatAgnosticService,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IMapper mapper,
        IMessageCacheManager messageCacheManager)
    {
        _messageAgnosticService = messageAgnosticService;
        _chatAgnosticService = chatAgnosticService;
        _mapper = mapper;
        _messageCacheManager = messageCacheManager;
        _kafkaSettings = kafkaSettings.Value;
        _linkPreviewNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
    }

    public async Task<Unit> Handle(SetLinkPreviewCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageAgnosticService.SetLinkPreviewAsync(
            request.MessageId!,
            request.Url!,
            request.Title!,
            request.ImageUrl!,
            cancellationToken);

        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId!, cancellationToken);
        var linkPreviewDto = _mapper.Map<LinkPreviewDto>(message);

        await Task.WhenAll([
            _linkPreviewNotificationProducer.ProduceAsync(
                _kafkaSettings.LinkPreviewNotification!.Topic,
                Guid.NewGuid().ToString(),
                accountIds.Select(accountId => new Notification<LinkPreviewDto>
                {
                    RecipientId = accountId,
                    Message = linkPreviewDto
                }).ToArray()),
            _messageCacheManager.ClearAsync(message.ChatId!)
        ]);

        return Unit.Value;
    }
}
