using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;

public class SetImagePreviewCommandHandler : IRequestHandler<SetImagePreviewCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService;
    private readonly IChatAgnosticService _chatAgnosticService;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _imagePreviewNotificationProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageCacheManager _messageCacheManager;

    public SetImagePreviewCommandHandler(
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
        _imagePreviewNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public async Task<Unit> Handle(SetImagePreviewCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageAgnosticService.SaveImagePreviewAsync(
            request.MessageId!,
            request.Filename!,
            request.ImageFormat,
            request.Width,
            request.Height,
            cancellationToken: cancellationToken);

        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId!, cancellationToken);
        var imagePreviewDto = _mapper.Map<ImagePreviewDto>(message);

        await Task.WhenAll([
            _imagePreviewNotificationProducer.ProduceAsync(
                _kafkaSettings.ImagePreviewNotification!.Topic,
                Guid.NewGuid().ToString(),
                accountIds.Select(accountId => new Notification<ImagePreviewDto>
                {
                    RecipientId = accountId,
                    Message = imagePreviewDto
                }).ToArray()),
            _messageCacheManager.ClearAsync(message.ChatId!)
        ]);

        return Unit.Value;
    }
}
