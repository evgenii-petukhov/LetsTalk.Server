using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

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
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync()
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task UpdateAsync(T entity)
    {
        //_context.Update(entity); // should work equally to the line below
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
