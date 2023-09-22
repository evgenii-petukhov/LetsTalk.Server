using MediatR;

namespace LetsTalk.Server.Domain;

public class BaseEntity
{
    private readonly List<INotification> _domainEvents = new();

    public int Id { get; protected set; }

    public BaseEntity(int id)
    {
        Id = id;
    }

    protected BaseEntity()
    {
    }

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
