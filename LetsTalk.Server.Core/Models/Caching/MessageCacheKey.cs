namespace LetsTalk.Server.Core.Models.Caching;

public struct MessageCacheKey
{
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
}
