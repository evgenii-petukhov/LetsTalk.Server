using LetsTalk.Server.LinkPreview.Abstractions;

namespace LetsTalk.Server.LinkPreview.Services;

public class DownloadService : IDownloadService
{
    public async Task<string?> DownloadAsString(string url)
    {
        using var client = new HttpClient();
        try
        {
            return await client.GetStringAsync(url)
                .ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }
}
