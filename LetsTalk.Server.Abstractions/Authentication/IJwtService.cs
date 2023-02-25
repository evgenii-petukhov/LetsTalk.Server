namespace LetsTalk.Server.Abstractions.Authentication;

public interface IJwtService
{
    public string GenerateJwtToken(int accountId);

    public int? ValidateJwtToken(string? token);
}