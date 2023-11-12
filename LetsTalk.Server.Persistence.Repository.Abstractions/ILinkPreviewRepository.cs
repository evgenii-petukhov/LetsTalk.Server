using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface ILinkPreviewRepository : IGenericRepository<LinkPreview>
{
    Task<int> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default);
}
