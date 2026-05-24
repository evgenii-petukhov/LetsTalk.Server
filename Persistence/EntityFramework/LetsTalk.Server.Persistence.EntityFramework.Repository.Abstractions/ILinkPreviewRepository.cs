using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface ILinkPreviewRepository : IGenericRepository<LinkPreview>
{
    Task<int> GetIdByUrlAsync(string url, CancellationToken cancellationToken = default);
}
