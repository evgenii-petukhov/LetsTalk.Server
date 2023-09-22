using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Persistence.Repository;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly LetsTalkDbContext _context;
    private readonly IMediator _mediator;
    private bool _disposedValue;

    public UnitOfWork(
        LetsTalkDbContext context,
        IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents?.Any() == true)
            .ToList();

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            var tasks = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .Select(domainEvent => _mediator.Publish(domainEvent, cancellationToken));
            await Task.WhenAll(tasks);
        }
        catch
        {
            _context.ChangeTracker.Clear();
            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
