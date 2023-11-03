namespace LetsTalk.Server.Caching;

public struct MessageCacheKey
{
    public int SenderId { get; set; }

    public int RecipientId { get; set; }
}
