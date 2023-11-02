namespace LetsTalk.Server.Core.Models;

public struct MessageCacheKey
{
    public int SenderId { get; set; }
    
    public int RecipientId { get; set; }
}
