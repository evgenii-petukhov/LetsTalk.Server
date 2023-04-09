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
using System.Web;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly IDownloadService _downloadService;
    private readonly IRegexService _regexService;
    private readonly ILogger<LinkPreviewRequest> _logger;
    private readonly ILinkPreviewRepository _linkPreviewRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly KafkaSettings _kafkaSettings;

    private readonly IMessageProducer _producer;

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
        _kafkaSettings = kafkaSettings.Value;
        _linkPreviewRepository = linkPreviewRepository;
        _messageRepository = messageRepository;

        _producer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url == null) return;

        var linkPreview = await _linkPreviewRepository.GetByUrlAsync(request.Url)
            .ConfigureAwait(false);

        if (linkPreview == null)
        {
            try
            {
                var pageString = await _downloadService.DownloadAsString(request.Url)
                    .ConfigureAwait(false);

                var openGraphModel = _regexService.GetOpenGraphModel(pageString);
                _logger.LogInformation("{@opModel}", openGraphModel);

                if (string.IsNullOrWhiteSpace(openGraphModel.Title))
                {
                    _logger.LogInformation("og:title not found: {url}", request.Url);
                    return;
                }

                openGraphModel.Title = HttpUtility.HtmlDecode(openGraphModel.Title);
                linkPreview = await _linkPreviewRepository.CreateAsync(new Domain.LinkPreview
                {
                    Url = request.Url,
                    Title = openGraphModel.Title,
                    ImageUrl = openGraphModel.ImageUrl
                }).ConfigureAwait(false);

                _logger.LogInformation("{@linkPreview}", linkPreview);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Unable to download: {url}", request.Url);
                return;
            }
        }
        else
        {
            _logger.LogInformation("Fetched from DB: {url}", request.Url);
        }

        await _messageRepository.SetLinkPreviewAsync(request.MessageId, linkPreview.Id)
            .ConfigureAwait(false);
        await SendNotification(request.RecipientId, request.SenderId, request.MessageId, linkPreview)
            .ConfigureAwait(false);
        await SendNotification(request.SenderId, request.RecipientId, request.MessageId, linkPreview)
            .ConfigureAwait(false);
    }

    private Task SendNotification(int recipientId, int senderId, int messageId, Domain.LinkPreview linkPreview)
    {
        return _producer.ProduceAsync(
            _kafkaSettings.LinkPreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<LinkPreviewDto>
            {
                RecipientId = recipientId,
                Message = new LinkPreviewDto
                {
                    AccountId = senderId,
                    MessageId = messageId,
                    Title = linkPreview.Title,
                    ImageUrl = linkPreview.ImageUrl,
                    Url = linkPreview.Url
                }
            });
    }
}
