using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.Infrastructure.ApiClient;
using MassTransit;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestConsumer(
    ILinkPreviewService linkPreviewService,
    ILogger<LinkPreviewRequestConsumer> logger,
    IHttpClientService httpClientService,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> applicationSettingsOptions,
    IOptions<LinkPreviewSettings> linkPreviewSettingsOptions) : IConsumer<LinkPreviewRequest>
{
    private static readonly Action<ILogger, string, Exception?> _logTitleEmpty =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(LinkPreviewRequestConsumer)), "Title is empty: {Url}");
    private static readonly Action<ILogger, string, Exception?> _logSuccess =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, nameof(LinkPreviewRequestConsumer)), "New LinkPreview added: {Url}");
    private static readonly Action<ILogger, string, Exception?> _logUnableToDownload =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(3, nameof(LinkPreviewRequestConsumer)), "Unable to download: {Url}");

    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;
    private readonly ILogger<LinkPreviewRequestConsumer> _logger = logger;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly IHttpClientService _httpClientService = httpClientService;
    private readonly ApplicationUrlSettings _applicationUrlSettings = applicationSettingsOptions.Value;
    private readonly LinkPreviewSettings _linkPreviewSettings = linkPreviewSettingsOptions.Value;

    public async Task Consume(ConsumeContext<LinkPreviewRequest> context)
    {
        if (string.IsNullOrWhiteSpace(context.Message.Url))
        {
            return;
        }

        var model = await _linkPreviewService.GenerateLinkPreviewAsync(new Utility.Abstractions.Models.LinkPreviewRequest
        {
            Url = context.Message.Url,
            SecretKey = _linkPreviewSettings.SecretKey
        });

        if (model == null)
        {
            _logTitleEmpty(_logger, context.Message.Url, null);
            return;
        }

        if (model.Error != null)
        {
            _logUnableToDownload(_logger, context.Message.Url, model.Error);
            return;
        }

        var payload = new SetLinkPreviewRequest
        {
            MessageId = context.Message.MessageId,
            ChatId = context.Message.ChatId,
            Url = context.Message.Url,
            Title = model.OpenGraphModel?.Title,
            ImageUrl = model.OpenGraphModel?.ImageUrl
        };
        _signPackageService.Sign(payload);
        using var client = _httpClientService.GetHttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {context.Message.Token}");
        var apiClient = new ApiClient(_applicationUrlSettings.Api, client);
        await apiClient.SetLinkPreviewAsync(payload, context.CancellationToken);
        _logSuccess(_logger, context.Message.Url, null);
    }
}
