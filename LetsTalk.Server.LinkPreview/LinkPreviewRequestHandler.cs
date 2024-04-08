using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewGenerator _linkPreviewGenerator;
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IChatAgnosticService _chatAgnosticService;
    private readonly IMapper _mapper;

    public LinkPreviewRequestHandler(
        ILinkPreviewGenerator linkPreviewGenerator,
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor,
        IChatAgnosticService chatAgnosticService,
        IMapper mapper)
    {
        _linkPreviewGenerator = linkPreviewGenerator;
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
        _chatAgnosticService = chatAgnosticService;
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

        var accountIds = await _chatAgnosticService.GetChatMemberAccountIdsAsync(request.ChatId!);

        await _producer.ProduceAsync(
            _kafkaSettings.LinkPreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            accountIds.Select(accountId => new Notification<LinkPreviewDto>
            {
                RecipientId = accountId,
                Message = linkPreviewDto
            }).ToArray());
    }
}
