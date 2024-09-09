using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Notifications.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewGenerator _linkPreviewGenerator;
    private readonly IMessageProducer _linkPreviewNotificationProducer;
    private readonly IMessageProducer _clearMessageCacheRequestProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMapper _mapper;

    public LinkPreviewRequestHandler(
        ILinkPreviewGenerator linkPreviewGenerator,
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor,
        IMapper mapper)
    {
        _linkPreviewGenerator = linkPreviewGenerator;
        _kafkaSettings = kafkaSettings.Value;
        _linkPreviewNotificationProducer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
        _clearMessageCacheRequestProducer = producerAccessor.GetProducer(_kafkaSettings.ClearMessageCacheRequest!.Producer);
        _mapper = mapper;
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return;
        }

        var message = await _linkPreviewGenerator.ProcessMessageAsync(request.MessageId!, request.Url);

        if (message == null)
        {
            return;
        }

        var linkPreviewDto = _mapper.Map<LinkPreviewDto>(message);

        await Task.WhenAll([
            _linkPreviewNotificationProducer.ProduceAsync(
                _kafkaSettings.LinkPreviewNotification!.Topic,
                Guid.NewGuid().ToString(),
                request.AccountIds!.Select(accountId => new Notification<LinkPreviewDto>
                {
                    RecipientId = accountId,
                    Message = linkPreviewDto
                }).ToArray()),
            _clearMessageCacheRequestProducer.ProduceAsync(_kafkaSettings.ClearMessageCacheRequest!.Topic,
                Guid.NewGuid().ToString(),
                new ClearMessageCacheRequest
                {
                    ChatId = request.ChatId
                })
        ]);
    }
}
