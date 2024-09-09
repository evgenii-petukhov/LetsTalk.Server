namespace LetsTalk.Server.API.Core.Abstractions;

public interface IProfileCacheManager
{
    Task ClearAsync(string accountId);
}
