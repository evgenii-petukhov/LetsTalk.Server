namespace LetsTalk.Server.Authentication.Abstractions;

public interface IAuthenticationClient
{
    Task<string> GenerateJwtTokenAsync(string url, int accountId);

    Task<int?> ValidateJwtTokenAsync(string url, string? token);
}
