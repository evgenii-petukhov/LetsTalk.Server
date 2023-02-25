using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Core.Contracts.Authentication;

public interface IJwtService
{
    public string GenerateJwtToken(Account account);

    public int? ValidateJwtToken(string? token);
}

