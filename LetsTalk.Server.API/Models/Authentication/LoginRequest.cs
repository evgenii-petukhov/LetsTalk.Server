namespace LetsTalk.Server.API.Models.Authentication;

public class LoginRequest
{
    public string? Provider { get; set; }

    public string? Id { get; set; }

    public string? AuthToken { get; set; }
}