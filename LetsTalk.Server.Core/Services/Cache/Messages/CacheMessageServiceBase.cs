namespace LetsTalk.Server.Core.Services.Cache.Messages;

public abstract class CacheMessageServiceBase
{
    protected static string GetMessagePageKey(int senderId, int recipientId)
    {
        return $"MessagePage:{senderId}:{recipientId}";
    }

    protected static string GetFirstMessagePageKey(int senderId, int recipientId)
    {
        return $"MessagePageFirst:{senderId}:{recipientId}";
    }
}
