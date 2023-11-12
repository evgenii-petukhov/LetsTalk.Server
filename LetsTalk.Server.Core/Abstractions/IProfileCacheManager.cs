namespace LetsTalk.Server.Core.Abstractions;

public interface IProfileCacheManager
{
    Task RemoveAsync(int accountId);
}
