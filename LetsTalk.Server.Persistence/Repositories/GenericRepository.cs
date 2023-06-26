using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;

namespace LetsTalk.Server.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly LetsTalkDbContext _context;

    public GenericRepository(LetsTalkDbContext context)
    {
        _context = context;
    }

    public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Add(entity);
        return _context.SaveChangesAsync(cancellationToken);
    }
}
