namespace LetsTalk.Server.API.Core.Abstractions;

public interface IAccountCacheManager
{
    Task ClearAsync();
}
