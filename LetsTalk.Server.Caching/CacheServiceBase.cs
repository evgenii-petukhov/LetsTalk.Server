namespace LetsTalk.Server.Caching;

public abstract class CacheServiceBase
{
    protected static string GetMessageKey(int senderId, int recipientId)
    {
        return $"Messages:{senderId}:{recipientId}";
    }
}
