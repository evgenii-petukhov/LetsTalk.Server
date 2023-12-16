namespace LetsTalk.Server.Core.Abstractions;

public interface IProfileCacheManager
{
    Task RemoveAsync(string accountId);
}
