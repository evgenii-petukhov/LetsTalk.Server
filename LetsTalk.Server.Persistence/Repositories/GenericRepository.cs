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

    public async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity)
            .ConfigureAwait(false);
        await _context.SaveChangesAsync()
            .ConfigureAwait(false);
        return entity;
    }
}
