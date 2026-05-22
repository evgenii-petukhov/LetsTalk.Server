namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface ILinkPreviewRepository
{
    Task<string?> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default);
}
