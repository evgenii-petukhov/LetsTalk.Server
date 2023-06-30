using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories;

public class FileRepository : GenericRepository<Domain.File>, IFileRepository
{
    public FileRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Files
            .Where(file => file.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
