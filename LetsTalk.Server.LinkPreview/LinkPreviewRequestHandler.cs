using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.LinkPreview.Abstractions;
using Microsoft.Extensions.Logging;
using LetsTalk.Server.Configuration.Models;
using KafkaFlow.Producers;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.LinkPreview.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly IDownloadService _downloadService;
    private readonly IRegexService _regexService;
    private readonly ILogger<LinkPreviewRequest> _logger;
    private readonly IProducerAccessor _producerAccessor;
    private readonly ILinkPreviewRepository _linkPreviewRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly KafkaSettings _kafkaSettings;

    public LinkPreviewRequestHandler(
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewRequest> logger,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        ILinkPreviewRepository linkPreviewRepository,
        IMessageRepository messageRepository)
    {
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
        _producerAccessor = producerAccessor;
        _kafkaSettings = kafkaSettings.Value;
        _linkPreviewRepository = linkPreviewRepository;
        _messageRepository = messageRepository;
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url == null) return;

        var pageString = await _downloadService.DownloadAsString(request.Url);
        if (string.IsNullOrEmpty(pageString))
        {
            Console.WriteLine("error");
        }
        else
        {
            var opModel = _regexService.GetOpenGraphModel(pageString);
            _logger.LogInformation("{@opModel}", opModel);

            if (!string.IsNullOrWhiteSpace(opModel.Title))
            {
                var producer = _producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
                var linkPreview = await _linkPreviewRepository.GetByUrlAsync(request.Url);
                linkPreview ??= await _linkPreviewRepository.CreateAsync(new Domain.LinkPreview
                    {
                        Url = request.Url,
                        Title = opModel.Title,
                        ImageUrl = opModel.ImageUrl
                    });
                await _messageRepository.SetLinkPreviewAsync(request.MessageId, linkPreview.Id);
                await producer.ProduceAsync(
                    _kafkaSettings.LinkPreviewNotification.Topic,
                    Guid.NewGuid().ToString(),
                    new Notification<LinkPreviewDto>
                    {
                        RecipientId = request.RecipientId,
                        Message = new LinkPreviewDto
                        {
                            AccountId = request.SenderId,
                            MessageId = request.MessageId,
                            Title = opModel.Title,
                            ImageUrl = opModel.ImageUrl
                        }
                    });
                await producer.ProduceAsync(
                    _kafkaSettings.LinkPreviewNotification.Topic,
                    Guid.NewGuid().ToString(),
                    new Notification<LinkPreviewDto>
                    {
                        RecipientId = request.SenderId,
                        Message = new LinkPreviewDto
                        {
                            AccountId = request.RecipientId,
                            MessageId = request.MessageId,
                            Title = opModel.Title,
                            ImageUrl = opModel.ImageUrl
                        }
                    });
            }
        }
    }
}
