using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories;

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
