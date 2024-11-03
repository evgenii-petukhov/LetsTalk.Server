using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using Microsoft.Extensions.Logging;
using LetsTalk.Server.SignPackage.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator(
    ILinkPreviewService linkPreviewService,
    ILogger<LinkPreviewGenerator> logger,
    IHttpClientFactory httpClientFactory,
    ISignPackageService signPackageService,
    IOptions<ApplicationUrlSettings> options) : LinkPreviewGeneratorBase(logger, httpClientFactory, signPackageService, options), ILinkPreviewGenerator
{
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;

    public async Task ProcessMessageAsync(string messageId, string chatId, string url)
    {
        var model = await _linkPreviewService.GenerateLinkPreviewAsync(url);

        await HandleResponseAsync(model, messageId, chatId, url);
    }
}
