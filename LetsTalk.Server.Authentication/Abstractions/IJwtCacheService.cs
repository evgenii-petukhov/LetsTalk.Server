namespace LetsTalk.Server.Authentication.Abstractions;

public interface IJwtCacheService
{
    Task<string?> GetAccountIdAsync(string? token);

    ValueTask<string> GenerateAsync(string accountId);
}
