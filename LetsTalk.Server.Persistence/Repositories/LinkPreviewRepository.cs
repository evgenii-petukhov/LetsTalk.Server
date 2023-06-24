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

    public Task<LinkPreview?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return _context.LinkPreviews
            .SingleOrDefaultAsync(q => q.Url == url, cancellationToken: cancellationToken);
    }
}
