using MediatR;

namespace LetsTalk.Server.Domain.Events;

public class MessageDomainEvent<T> : INotification where T: BaseEntity
{
    public Message? Message { get; set; }

    public T? Payload { get; set; }
}
