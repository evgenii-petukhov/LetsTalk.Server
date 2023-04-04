using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using Microsoft.Extensions.Logging;
using LetsTalk.Server.Configuration.Models;
using static Confluent.Kafka.ConfigPropertyNames;
using KafkaFlow.Producers;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly IDownloadService _downloadService;
    private readonly IRegexService _regexService;
    private readonly ILogger<LinkPreviewRequest> _logger;
    private readonly IProducerAccessor _producerAccessor;
    private readonly KafkaSettings _kafkaSettings;

    public LinkPreviewRequestHandler(
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewRequest> logger,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
        _producerAccessor = producerAccessor;
        _kafkaSettings = kafkaSettings.Value;
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
            var m = _regexService.GetOpenGraphModel(pageString);
            _logger.LogInformation("{@m}", m);

            if (!string.IsNullOrWhiteSpace(m.Title))
            {
                var producer = _producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
                _ = producer.ProduceAsync(
                    _kafkaSettings.LinkPreviewNotification.Topic,
                    Guid.NewGuid().ToString(),
                    new LinkPreviewNotification
                    {
                        RecipientId = request.RecipientId,
                        MessageId = request.MessageId,
                        Title = m.Title,
                        ImageUrl = m.ImageUrl
                    });
            }
        }
    }
}
