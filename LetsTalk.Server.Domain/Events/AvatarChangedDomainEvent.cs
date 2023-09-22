using MediatR;

namespace LetsTalk.Server.Domain.Events;

public class AvatarChangedDomainEvent :  INotification
{
    public int PreviousImageId { get; set; }
}
