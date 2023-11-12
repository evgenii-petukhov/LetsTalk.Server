namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface ILinkPreviewAgnosticService
{
    Task<int> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default);
}
