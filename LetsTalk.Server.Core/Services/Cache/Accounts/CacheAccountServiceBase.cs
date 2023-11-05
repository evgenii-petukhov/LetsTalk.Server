namespace LetsTalk.Server.Core.Services.Cache.Messages;

public abstract class CacheAccountServiceBase
{
    protected static string GetContactsKey(int accountId)
    {
        return $"Contacts:{accountId}";
    }

    protected static string GetProfileKey(int accountId)
    {
        return $"Profile:{accountId}";
    }
}
