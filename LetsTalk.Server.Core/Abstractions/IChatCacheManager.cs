namespace LetsTalk.Server.Core.Abstractions;

public interface IChatCacheManager
{
    Task RemoveAsync(string accountId);
}
