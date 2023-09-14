namespace LetsTalk.Server.Authentication.Abstractions;

public interface IJwtService
{
    string GenerateJwtToken(int accountId);

    ValueTask<int?> ValidateJwtTokenAsync(string? token);
}