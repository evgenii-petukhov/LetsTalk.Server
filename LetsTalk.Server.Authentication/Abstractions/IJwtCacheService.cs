namespace LetsTalk.Server.Authentication.Abstractions;

public interface IJwtCacheService
{
    Task<int?> GetAccountIdAsync(string? token);

    Task<string> GenerateAsync(int accountId);
}
