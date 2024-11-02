using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Utility.Services;

public class DownloadService(IHttpClientFactory httpClientFactory) : IDownloadService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<string> DownloadAsStringAsync(string url, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient(nameof(DownloadService));
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        return await client.GetStringAsync(url, cancellationToken);
    }
}
