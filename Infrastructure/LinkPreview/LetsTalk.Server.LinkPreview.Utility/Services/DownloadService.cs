using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Utility.Services;

public class DownloadService(IHttpClientService httpClientService) : IDownloadService
{
    private readonly IHttpClientService _httpClientService = httpClientService;

    public async Task<string> DownloadAsStringAsync(string url, CancellationToken cancellationToken)
    {
        using var client = _httpClientService.GetHttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        return await client.GetStringAsync(url, cancellationToken);
    }
}
