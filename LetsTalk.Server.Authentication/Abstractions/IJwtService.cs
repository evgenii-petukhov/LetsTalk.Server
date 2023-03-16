namespace LetsTalk.Server.Authentication.Abstractions;

public interface IJwtService
{
    public string GenerateJwtToken(int accountId);

    public int? ValidateJwtToken(string? token);
}