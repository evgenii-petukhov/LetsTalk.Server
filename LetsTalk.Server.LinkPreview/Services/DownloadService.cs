using LetsTalk.Server.LinkPreview.Abstractions;

namespace LetsTalk.Server.LinkPreview.Services;

public class DownloadService : IDownloadService
{
    public async Task<string> DownloadAsStringAsync(string url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Other");
        return await client.GetStringAsync(url);
    }
}
