using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewGenerator _linkPreviewGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntityFactory _entityFactory;
    private readonly KafkaSettings _kafkaSettings;

    private readonly IMessageProducer _producer;

    public LinkPreviewRequestHandler(
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        ILinkPreviewGenerator linkPreviewGenerator,
        IUnitOfWork unitOfWork,
        IEntityFactory entityFactory)
    {
        _kafkaSettings = kafkaSettings.Value;
        _linkPreviewGenerator = linkPreviewGenerator;
        _unitOfWork = unitOfWork;
        _entityFactory = entityFactory;
        _producer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url == null)
        {
            return;
        }

        var linkPreview = await _linkPreviewGenerator.GetLinkPreviewAsync(request.Url);

        if (linkPreview == null)
        {
            return;
        }

        var message = _entityFactory.CreateMessage(request.MessageId);
        message.SetLinkPreview(linkPreview);
        await _unitOfWork.SaveAsync();

        var linkPreviewDto = new LinkPreviewDto
        {
            MessageId = request.MessageId,
            Title = linkPreview.Title,
            ImageUrl = linkPreview.ImageUrl,
            Url = linkPreview.Url
        };

        await _producer.ProduceAsync(
            _kafkaSettings.LinkPreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<LinkPreviewDto>[]
            {
                new Notification<LinkPreviewDto>
                {
                    RecipientId = request.RecipientId,
                    Message = linkPreviewDto with
                    {
                        AccountId = request.SenderId
                    }
                },
                new Notification<LinkPreviewDto>
                {
                    RecipientId = request.SenderId,
                    Message = linkPreviewDto with
                    {
                        AccountId = request.RecipientId
                    }
                }
            });
    }
}
