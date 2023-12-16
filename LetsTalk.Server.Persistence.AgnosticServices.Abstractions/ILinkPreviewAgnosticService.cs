namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface ILinkPreviewAgnosticService
{
    Task<string?> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default);
}
