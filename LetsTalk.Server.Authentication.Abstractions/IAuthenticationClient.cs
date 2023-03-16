namespace LetsTalk.Server.Authentication.Abstractions;

public interface IAuthenticationClient
{
    Task<string> GenerateJwtToken(string url, int accountId);

    Task<int?> ValidateJwtToken(string url, string? token);
}
