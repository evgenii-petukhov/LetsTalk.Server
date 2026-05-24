namespace LetsTalk.Server.API.Core.Abstractions;

public interface IChatCacheManager
{
    Task ClearAsync(string accountId);
}
