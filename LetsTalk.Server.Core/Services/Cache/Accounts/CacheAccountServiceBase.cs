namespace LetsTalk.Server.Core.Services.Cache.Messages;

public abstract class CacheAccountServiceBase
{
    protected static string GetContactsKey(int accountId)
    {
        return $"account:{accountId}:contacts";
    }

    protected static string GetProfileKey(int accountId)
    {
        return $"account:{accountId}:profile";
    }
}
