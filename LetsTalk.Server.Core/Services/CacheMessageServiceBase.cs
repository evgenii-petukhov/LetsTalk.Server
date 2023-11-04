namespace LetsTalk.Server.Core.Services;

public abstract class CacheMessageServiceBase
{
    protected static string GetMessageKey(int senderId, int recipientId)
    {
        return $"Messages:{senderId}:{recipientId}";
    }
}
