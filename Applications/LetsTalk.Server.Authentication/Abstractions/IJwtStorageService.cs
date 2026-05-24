using LetsTalk.Server.Authentication.Models;

namespace LetsTalk.Server.Authentication.Abstractions;

public interface IJwtStorageService
{
    Task<StoredToken?> GetStoredTokenAsync(string? token);

    StoredToken Generate(string accountId);
}
