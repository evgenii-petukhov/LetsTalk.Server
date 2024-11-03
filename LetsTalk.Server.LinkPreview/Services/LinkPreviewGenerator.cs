using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator(
    ILinkPreviewService linkPreviewService,
    ILogger<LinkPreviewGenerator> logger,
    IHttpClientFactory httpClientFactory,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> options) : ILinkPreviewGenerator, IDisposable
{
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;
    private readonly ILogger<LinkPreviewGenerator> _logger = logger;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(LinkPreviewService));
    private readonly ApplicationUrlSettings _applicationUrlSettings = options.Value;
    private bool _disposedValue;

    public async Task ProcessMessageAsync(string messageId, string chatId, string url)
    {
        var model = await _linkPreviewService.GenerateLinkPreviewAsync(url);

        if (model == null)
        {
            _logger.LogInformation("Title is empty: {url}", url);
            return;
        }

        if (model.Exception != null)
        {
            _logger.LogError(model.Exception, "Unable to download: {url}", url);
        }

        try
        {
            var request = new SetLinkPreviewRequest
            {
                MessageId = messageId,
                ChatId = chatId,
                Url = url,
                Title = model.OpenGraphModel!.Title,
                ImageUrl = model.OpenGraphModel!.ImageUrl
            };
            _signPackageService.Sign(request);
            using var content = GetHttpContent(request);
            await _httpClient.PutAsync($"{_applicationUrlSettings.Api}/api/message/setlinkpreview", content);
            _logger.LogInformation("New LinkPreview added: {url}", url);
        }
        catch
        {
            var request = new SetExistingLinkPreviewRequest
            {
                MessageId = messageId,
                ChatId = chatId,
                Url = url
            };
            _signPackageService.Sign(request);
            using var content = GetHttpContent(request);
            await _httpClient.PutAsync($"{_applicationUrlSettings.Api}/api/message/setexistinglinkPreview", content);
            _logger.LogInformation("Fetched from DB: {url}", url);
        }
    }

    private HttpContent GetHttpContent(object payload)
    {
        return new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
