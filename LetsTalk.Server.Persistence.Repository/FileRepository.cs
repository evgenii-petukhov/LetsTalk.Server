using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repository;

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
