namespace LetsTalk.Server.Core.Services.Cache.Messages;

public abstract class CacheMessageServiceBase
{
    protected static string GetMessagePageKey(int senderId, int recipientId)
    {
        return $"messages:{senderId}:{recipientId}";
    }

    protected static string GetFirstMessagePageKey(int senderId, int recipientId)
    {
        return $"messages:{senderId}:{recipientId}:first-page";
    }
}
