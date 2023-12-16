namespace LetsTalk.Server.Authentication.Abstractions;

public interface IAuthenticationClient
{
    Task<string> GenerateJwtTokenAsync(string accountId);

    Task<string?> ValidateJwtTokenAsync(string token);
}
