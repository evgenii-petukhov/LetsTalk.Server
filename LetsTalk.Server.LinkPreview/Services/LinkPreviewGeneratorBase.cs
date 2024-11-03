using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

namespace LetsTalk.Server.LinkPreview.Services;

public abstract class LinkPreviewGeneratorBase(
    ILogger<LinkPreviewGenerator> logger,
    IHttpClientFactory httpClientFactory,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> options) : IDisposable
{
    private readonly ILogger<LinkPreviewGenerator> _logger = logger;
    private readonly ISignPackageService _signPackageService = signPackageService;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(LinkPreviewGeneratorBase));
    private readonly ApplicationUrlSettings _applicationUrlSettings = options.Value;
    private bool _disposedValue;

    protected async Task HandleResponseAsync(LinkPreviewResponse? model, string messageId, string chatId, string url)
    {
        if (model == null)
        {
            _logger.LogInformation("Title is empty: {url}", url);
            return;
        }

        if (model.Error != null)
        {
            _logger.LogError(model.Error, "Unable to download: {url}", url);
        }

        try
        {
            await SendPutRequestAsync($"{_applicationUrlSettings.Api}/api/message/setlinkpreview", new SetLinkPreviewRequest
            {
                MessageId = messageId,
                ChatId = chatId,
                Url = url,
                Title = model.OpenGraphModel!.Title,
                ImageUrl = model.OpenGraphModel!.ImageUrl
            });
            _logger.LogInformation("New LinkPreview added: {url}", url);
        }
        catch
        {
            await SendPutRequestAsync($"{_applicationUrlSettings.Api}/api/message/setexistinglinkPreview", new SetExistingLinkPreviewRequest
            {
                MessageId = messageId,
                ChatId = chatId,
                Url = url
            });
            _logger.LogInformation("Fetched from DB: {url}", url);
        }
    }

    private async Task SendPutRequestAsync(string url, object payload)
    {
        _signPackageService.Sign(payload);
        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");
        await _httpClient.PutAsync(url, content);
    }

    protected virtual void Dispose(bool disposing)
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
