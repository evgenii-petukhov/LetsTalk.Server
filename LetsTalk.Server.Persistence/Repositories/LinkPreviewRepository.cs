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

    public async Task<LinkPreview?> GetByUrlAsync(string url)
    {
        return await _context.Set<LinkPreview>()
            .AsNoTracking()
            .SingleOrDefaultAsync(q => q.Url == url);
    }
}
