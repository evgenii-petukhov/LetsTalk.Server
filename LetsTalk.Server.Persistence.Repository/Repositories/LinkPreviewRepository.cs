using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repository.Repositories;

public class LinkPreviewRepository : GenericRepository<LinkPreview>, ILinkPreviewRepository
{
    public LinkPreviewRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task<LinkPreview?> GetByUrlOrDefaultAsync(string url, CancellationToken cancellationToken = default)
    {
        return _context.LinkPreviews
            .FirstOrDefaultAsync(q => q.Url == url, cancellationToken);
    }
}
