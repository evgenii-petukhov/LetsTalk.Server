using LetsTalk.Server.LinkPreview.Utility.Abstractions;

namespace LetsTalk.Server.LinkPreview.Utility.Services;

public class LambdaDownloadService() : IDownloadService
{
    public async Task<string> DownloadAsStringAsync(string url, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        return await client.GetStringAsync(url, cancellationToken);
    }
}
