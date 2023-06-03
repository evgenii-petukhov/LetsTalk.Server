using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface ILinkPreviewRepository : IGenericRepository<LinkPreview>
{
    Task<LinkPreview?> GetByUrlAsync(string url, CancellationToken cancellationToken = default);
}
