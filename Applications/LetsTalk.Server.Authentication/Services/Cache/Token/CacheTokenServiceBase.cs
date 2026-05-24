namespace LetsTalk.Server.Authentication.Services.Cache.Token;

public class CacheTokenServiceBase
{
    protected static string GetTokenKey(string token)
    {
        return $"jwt:{token}";
    }
}
