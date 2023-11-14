using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository.Repositories;

public class FileRepository : GenericRepository<Domain.File>, IFileRepository
{
    public FileRepository(LetsTalkDbContext context) : base(context)
    {
    }
}
