using AutoMapper;
using KafkaFlow.Producers;
using KafkaFlow;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetExistingLinkPreview;

public class SetExistingLinkPreviewCommandHandler : IRequestHandler<SetExistingLinkPreviewCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService;
    private readonly ILinkPreviewAgnosticService _linkPreviewAgnosticService;
    private readonly IChatAgnosticService _chatAgnosticService;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _linkPreviewNotificationProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageCacheManager _messageCacheManager;

    public SetExistingLinkPreviewCommandHandler(
        IMessageAgnosticService messageAgnosticService,
        ILinkPreviewAgnosticService linkPreviewAgnosticService,
        IChatAgnosticService chatAgnosticService,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IMapper mapper,
        IMessageCacheManager messageCacheManager)
    {
        _messageAgnosticService = messageAgnosticService;
        _linkPreviewAgnosticService = linkPreviewAgnosticService;
        _chatAgnosticService = chatAgnosticService;
        _mapper = mapper;
        _messageCacheManager = messageCacheManager;
        _kafkaSettings = kafkaSettings.Value;
        _linkPreviewNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
    }

    public async Task<Unit> Handle(SetExistingLinkPreviewCommand request, CancellationToken cancellationToken)
    {
        var linkPreviewId = await _linkPreviewAgnosticService.GetIdByUrlAsync(request.Url!, cancellationToken);

        var message = await _messageAgnosticService.SetLinkPreviewAsync(
            request.MessageId!,
            linkPreviewId!,
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
