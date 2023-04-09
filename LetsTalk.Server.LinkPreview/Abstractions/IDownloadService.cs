namespace LetsTalk.Server.LinkPreview.Abstractions;

public interface IDownloadService
{
    public Task<string> DownloadAsString(string url);
}
