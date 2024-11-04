using KafkaFlow;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.Infrastructure.ApiClient;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler(
    ILinkPreviewService linkPreviewService,
    ILogger<LinkPreviewRequestHandler> logger,
    IHttpClientService httpClientService,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> options) : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;
    private readonly ILogger<LinkPreviewRequestHandler> _logger = logger;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly IHttpClientService _httpClientService = httpClientService;
    private readonly ApplicationUrlSettings _applicationUrlSettings = options.Value;

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return;
        }

        var model = await _linkPreviewService.GenerateLinkPreviewAsync(request.Url);

        if (model == null)
        {
            _logger.LogInformation("Title is empty: {url}", request.Url);
            return;
        }

        if (model.Error != null)
        {
            _logger.LogError(model.Error, "Unable to download: {url}", request.Url);
        }

        var payload = new SetLinkPreviewRequest
        {
            MessageId = request.MessageId,
            ChatId = request.ChatId,
            Url = request.Url,
            Title = model.OpenGraphModel!.Title,
            ImageUrl = model.OpenGraphModel!.ImageUrl
        };
        _signPackageService.Sign(payload);
        using var client = _httpClientService.GetHttpClient();
        var apiClient = new ApiClient(_applicationUrlSettings.Api, client);
        await apiClient.SetLinkPreviewAsync(payload);
        _logger.LogInformation("New LinkPreview added: {url}", request.Url);
    }
}
