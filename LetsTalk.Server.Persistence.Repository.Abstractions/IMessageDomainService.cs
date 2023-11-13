namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IMessageDomainService
{
    void MarkAsRead(int messageId);

    Task MarkAllAsRead(int recipientId, int messageId, CancellationToken cancellationToken);
}
