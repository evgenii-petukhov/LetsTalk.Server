namespace LetsTalk.Server.Core.Abstractions;

public interface IAccountCacheManager
{
    Task RemoveAsync(int accountId);
}
