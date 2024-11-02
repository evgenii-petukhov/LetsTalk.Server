namespace LetsTalk.Server.LinkPreview.Utility.Abstractions;

public interface IDownloadService
{
    public Task<string> DownloadAsStringAsync(string url, CancellationToken cancellationToken);
}
