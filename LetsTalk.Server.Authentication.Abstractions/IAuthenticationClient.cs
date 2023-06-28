namespace LetsTalk.Server.Authentication.Abstractions;

public interface IAuthenticationClient
{
    Task<string> GenerateJwtTokenAsync(int accountId);

    Task<int?> ValidateJwtTokenAsync(string token);
}
