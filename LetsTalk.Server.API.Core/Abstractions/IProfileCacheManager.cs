namespace LetsTalk.Server.API.Core.Abstractions;

public interface IProfileCacheManager
{
    Task RemoveAsync(string accountId);
}
