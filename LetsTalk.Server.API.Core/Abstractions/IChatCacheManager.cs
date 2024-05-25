namespace LetsTalk.Server.API.Core.Abstractions;

public interface IChatCacheManager
{
    Task RemoveAsync(string accountId);
}
