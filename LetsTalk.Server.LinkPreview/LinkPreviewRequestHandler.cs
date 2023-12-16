using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.TypedHandler;
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
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;

    public LinkPreviewRequestHandler(
        ILinkPreviewGenerator linkPreviewGenerator,
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor)
    {
        _linkPreviewGenerator = linkPreviewGenerator;
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
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

        var linkPreviewDto = new LinkPreviewDto
        {
            MessageId = message.Id,
            Title = message.LinkPreview!.Title,
            ImageUrl = message.LinkPreview!.ImageUrl,
            Url = message.LinkPreview!.Url
        };

        await _producer.ProduceAsync(
            _kafkaSettings.LinkPreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<LinkPreviewDto>[]
            {
                new()
                {
                    RecipientId = message.RecipientId,
                    Message = linkPreviewDto with
                    {
                        AccountId = message.SenderId
                    }
                },
                new()
                {
                    RecipientId = message.SenderId,
                    Message = linkPreviewDto with
                    {
                        AccountId = message.RecipientId
                    }
                }
            });
    }
}
