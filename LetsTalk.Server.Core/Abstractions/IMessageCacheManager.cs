namespace LetsTalk.Server.Core.Abstractions;

public interface IMessageCacheManager
{
    Task RemoveAsync(int senderId, int recipientId);
}
