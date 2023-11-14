namespace LetsTalk.Server.Authentication.Abstractions;

public interface IJwtCacheService
{
    Task<string?> GetAccountIdAsync(string? token);

    Task<string> GenerateAsync(string accountId);
}
