namespace LetsTalk.Server.Authentication.Services.Cache;

public class CacheTokenServiceBase
{
    protected static string GetTokenKey(string token)
    {
        return $"jwt:{token}";
    }
}
